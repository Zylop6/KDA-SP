/*
 * Copyright (C) 2023 Zylop6
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
 * published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
 * See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with this program. If not, see <http://www.gnu.org/licenses/>.
 */



using System;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using WMPLib;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KDA_SP
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);
        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(
            IntPtr hdcDest,
            int xDest,
            int yDest,
            int width,
            int height,
            IntPtr hdcSrc,
            int xSrc,
            int ySrc,
            int dwRop
        );

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);
        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
        private bool isScanning = false;
        private bool AreaSelected;
        private string KillSoundPath;
        private string AssistSoundPath;
        private string DeathSoundPath;
        private System.Windows.Forms.Timer CaptureTimer;
        private Rectangle selectionRectangle;
        private bool isDrawingRectangle;
        private bool isExtended = false;
        private bool filteredPreview = false;
        private bool areaSelected = false;
        private Form selectionForm;
        private Point initialPosition;
        private Bitmap OCRBitmap;
        private string RawOCRText;
        private string ocrRaw;
        private string CleanOCRText;
        private int KillCount;
        private int DeathCount;
        private int AssistCount;
        private int OldKillCount = 0;
        private int OldDeathCount = 0;
        private int OldAssistCount = 0;
        private int ScanningInterval = 125;
        private double KDAR;
        private double KDR;
        private string KillCountText;
        private string DeathCountText;
        private string AssistCountText;
        private PerformanceMonitor performanceMonitor;
        private WindowsMediaPlayer mediaPlayer = new WindowsMediaPlayer();
        public class PerformanceMonitor
        {
            private PerformanceCounter memoryCounter;
            private PerformanceCounter cpuCounter;
            public PerformanceMonitor()
            {
                memoryCounter = new PerformanceCounter(
                    "Process",
                    "Working Set - Private",
                    Process.GetCurrentProcess().ProcessName
                );
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            }

            public void PerformanceMonitoring(Label memoryUsageLabel, Label cpuUsageLabel)
            {
                float memoryUsage = memoryCounter.NextValue() / (1024 * 1024); // Arbeitsspeicher in Megabytes
                float cpuUsage = cpuCounter.NextValue();

                memoryUsageLabel.Text = $"Memory Usage: {memoryUsage:F2} MB";
                cpuUsageLabel.Text = $"CPU Usage: {cpuUsage:F2}%";
            }
        }

        public MainForm()
        {
            InitializeComponent();
            CaptureTimer = new Timer();
            StartButton.Enabled = false;
            StopButton.Enabled = false;
            StartButton.BackColor = Color.DarkGray;
            StopButton.BackColor = Color.DarkGray;
            TestKillSoundButton.Enabled = false;
            TestDeathSoundButton.Enabled = false;
            TestAssistSoundButton.Enabled = false;
            this.Size = new Size(1132, 360);
            this.Icon = Properties.Resources.KDA_SP_ICON;
            performanceMonitor = new PerformanceMonitor();
            ScanIntervalScale.Value = 125;
        }
        //==========================================================================================================
        //-------------- Main  --------------
        private void InitializeCaptureTimer()
        {
            CaptureTimer.Interval = ScanIntervalScale.Value;
            CaptureTimer.Tick += new EventHandler(Refresh);
            CaptureTimer.Enabled = true;
        }

        private void UpdatePreviewPictureBox()
        {
            if (areaSelected == true)
            {
                if (filteredPreview == true)
                {
                    OCRBitmap = OCRCaptureScreen(selectionRectangle);
                    Image FilteredOCRBitmap = OCRBitmap;
                    FilteredOCRBitmap = ToGrayScale(FilteredOCRBitmap);
                    FilteredOCRBitmap = InvertImage(FilteredOCRBitmap);
                    FilteredOCRBitmap = ToBlackWhite(FilteredOCRBitmap);
                    SelectedAreaPicturebox.Image = FilteredOCRBitmap;
                }
                else
                {
                    OCRBitmap = OCRCaptureScreen(selectionRectangle);
                    SelectedAreaPicturebox.Image = OCRBitmap;
                }
            }
            else
            {
                SelectedAreaPicturebox.Image = null;
            }
        }
        private void Refresh(object sender, EventArgs e)
        {
            UpdatePreviewPictureBox();
            OCRBitmap = OCRCaptureScreen(selectionRectangle);

            Task.Run(() =>
            {
                if (isScanning)
                {
                    PerformOCR(sender, e);
                }
            });
            SoundPlayer();
            Monitoring();
        }
        private void PerformOCR(object sender, EventArgs e)
        {
            using (var imageStream = new System.IO.MemoryStream())
            {
                Bitmap OCRBitmap = OCRCaptureScreen(selectionRectangle);
                OCRBitmap = ToGrayScale(OCRBitmap);
                OCRBitmap = InvertImage(OCRBitmap);
                OCRBitmap = ToBlackWhite(OCRBitmap);

                OCRBitmap.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                imageStream.Seek(0, System.IO.SeekOrigin.Begin);

                var tempImagePath = System.IO.Path.GetTempFileName();
                using (var tempImage = System.Drawing.Image.FromStream(imageStream))
                {
                    tempImage.Save(tempImagePath, System.Drawing.Imaging.ImageFormat.Png);
                }

                using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(tempImagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            var extractedText = page.GetText();

                            if (!string.IsNullOrWhiteSpace(extractedText))
                            {
                                RawOCRText = extractedText;
                                Console.WriteLine(extractedText);
                            }
                        }
                    }
                }

                System.IO.File.Delete(tempImagePath);
            }
        }
        private void Monitoring()
        {
            if (RawOCRText != null)
            {
                string[] RawKDA = Regex.Replace(RawOCRText, @"[^0-9/]", "").Split('/');
                if (RawKDA != null && RawKDA.Length >= 3)
                {
                    if (int.TryParse(RawKDA[0], out KillCount))
                    {
                        KillCount = int.Parse(RawKDA[0]);
                        KillCountText = KillCount.ToString();
                    }
                    else
                    {
                        KillCountText = "?";
                    }

                    if (int.TryParse(RawKDA[1], out DeathCount))
                    {
                        DeathCount = int.Parse(RawKDA[1]);
                        DeathCountText = DeathCount.ToString();
                    }
                    else
                    {
                        DeathCountText = "?";
                    }

                    if (int.TryParse(RawKDA[2], out AssistCount))
                    {
                        AssistCount = int.Parse(RawKDA[2]);
                        AssistCountText = AssistCount.ToString();
                    }
                    else
                    {
                        AssistCountText = "?";
                    }
                    AssistCountLabel.Text = AssistCountText;
                    DeathCountLabel.Text = DeathCountText;
                    KillCountLabel.Text = KillCountText;

                    if (DeathCount != 0)
                    {
                        KDAR = Math.Round((double)(KillCount + AssistCount) / DeathCount, 2);
                        KDARLabel.Text = KDAR.ToString();

                        KDR = Math.Round((double)KillCount / DeathCount, 2);
                        KDRLabel.Text = KDR.ToString();
                    }
                    else
                    {
                        KDAR = Math.Round((double)(KillCount + AssistCount), 2);
                        KDARLabel.Text = KDAR.ToString();
                        KDRLabel.Text = KillCount.ToString();
                    }

                    if (isExtended == true)
                    {
                        CleanOCRText = $"K: {KillCountText} D: {DeathCountText}  A: {AssistCountText}";
                        CleanOutputLabel.Text = CleanOCRText;
                    }
                }
                if (isExtended == true)
                {
                    ocrRaw = RawOCRText
                        .Replace(Environment.NewLine, " ")
                        .Replace("\n", " ")
                        .Replace("\r", " ")
                        .Replace("\t", " ")
                        .Replace("@", "");
                    if (ocrRaw.Length > 30)
                    {
                        ocrRaw = ocrRaw.Substring(0, 30) + "...";
                    }
                    RawOutputLabel.Text = ocrRaw;
                }
            }

            if (isExtended == true)
            {
                performanceMonitor.PerformanceMonitoring(MemoryUsageLabel, CPUUsageLabel);
            }
        }
        private Bitmap SelectionBitmap()
        {
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            IntPtr screenDC = GetDC(IntPtr.Zero);
            IntPtr memoryDC = CreateCompatibleDC(screenDC);
            IntPtr bitmap = CreateCompatibleBitmap(
                screenDC,
                screenBounds.Width,
                screenBounds.Height
            );
            IntPtr oldBitmap = SelectObject(memoryDC, bitmap);

            BitBlt(
                memoryDC,
                0,
                0,
                screenBounds.Width,
                screenBounds.Height,
                screenDC,
                0,
                0,
                0x00CC0020
            );

            SelectObject(memoryDC, oldBitmap);
            DeleteDC(memoryDC);
            DeleteObject(screenDC);

            return Bitmap.FromHbitmap(bitmap);
        }
        private Bitmap OCRCaptureScreen(Rectangle captureRectangle)
        {
            IntPtr screenDC = GetDC(IntPtr.Zero);
            IntPtr memoryDC = CreateCompatibleDC(screenDC);
            IntPtr bitmap = CreateCompatibleBitmap(
                screenDC,
                captureRectangle.Width,
                captureRectangle.Height
            );
            IntPtr oldBitmap = SelectObject(memoryDC, bitmap);

            BitBlt(
                memoryDC,
                0,
                0,
                captureRectangle.Width,
                captureRectangle.Height,
                screenDC,
                captureRectangle.Left,
                captureRectangle.Top,
                0x00CC0020
            );

            SelectObject(memoryDC, oldBitmap);
            DeleteDC(memoryDC);
            ReleaseDC(IntPtr.Zero, screenDC);

            Bitmap resultBitmap = null;
            try
            {
                resultBitmap = (Bitmap)Image.FromHbitmap(bitmap);
                // Clone the bitmap to ensure it can be safely disposed of outside this method
                return (Bitmap)resultBitmap.Clone();
            }
            finally
            {
                // Dispose of the resultBitmap if it was created, to release GDI+ resources
                resultBitmap?.Dispose();
                DeleteObject(bitmap);
            }
        }

        //==========================================================================================================
        //-------------- Interactions  --------------
        //Check boxes
        private void MuteAllCheckChanged(object sender, EventArgs e)
        {
            if (MuteAllCheckBox.Checked == true)
            {
                KillVolumeSlider.Enabled = false;
                DeathVolumeSlider.Enabled = false;
                AssistVolumeSlider.Enabled = false;
                TestKillSoundButton.Enabled = false;
                TestDeathSoundButton.Enabled = false;
                TestAssistSoundButton.Enabled = false;
            }
            else if (MuteAllCheckBox.Checked == false)
            {
                KillVolumeSlider.Enabled = true;
                DeathVolumeSlider.Enabled = true;
                AssistVolumeSlider.Enabled = true;

                if (KillSoundPath != null)
                {
                    TestKillSoundButton.Enabled = true;
                }

                if (DeathSoundPath != null)
                {
                    TestDeathSoundButton.Enabled = true;
                }

                if (AssistSoundPath != null)
                {
                    TestAssistSoundButton.Enabled = true;
                }
            }
        }

        //Buttons
        private void StartButtonClick(object sender, EventArgs e)
        {
            isScanning = true;

            StopButton.BackColor = Color.FromArgb(255, 0, 50);
            StopButton.Enabled = true;

            StartButton.BackColor = Color.DarkGray;
            StartButton.Enabled = false;

            SelectDetectionAreaButton.BackColor = Color.DarkGray;
            SelectDetectionAreaButton.Enabled = false;

            InitializeCaptureTimer();
            CaptureTimer.Enabled = true;
        }
        private void StopButtonClick(object sender, EventArgs e)
        {
            isScanning = false;
            StopButton.BackColor = Color.DarkGray;
            StopButton.Enabled = false;

            StartButton.BackColor = Color.FromArgb(15, 250, 150);
            StartButton.Enabled = true;

            SelectDetectionAreaButton.BackColor = Color.FromArgb(125, 102, 204);
            SelectDetectionAreaButton.Enabled = true;

            CaptureTimer.Enabled = false;
        }
        private void ExpandExtraButtonClick(object sender, EventArgs e)
        {
            if (isExtended == false)
            {
                isExtended = true;
                ExpandExtraButton.BackgroundImage = Properties.Resources.Colapse_icon;
                this.MaximumSize = new Size(1574, 493);
                this.MinimumSize = new Size(1574, 493);
                this.Size = new Size(1578, 493);
            }
            else
            {
                isExtended = false;
                ExpandExtraButton.BackgroundImage = Properties.Resources.Expand_icon;
                this.MaximumSize = new Size(1132, 360);
                this.MinimumSize = new Size(1132, 360);
                this.Size = new Size(1132, 360);
            }
        }
        private void FilteredPreviewButtonClick(object sender, EventArgs e)
        {
            if (filteredPreview == false)
            {
                filteredPreview = true;
                FilteredPreviewButton.BackgroundImage = Properties.Resources.FilterOnIcon;
                UpdatePreviewPictureBox();
            }
            else
            {
                filteredPreview = false;
                FilteredPreviewButton.BackgroundImage = Properties.Resources.FilterOffIcon;
                UpdatePreviewPictureBox();
            }
        }

        //Area election button
        private void SelectDetectionAreaButtonClick(object sender, EventArgs e)
        {
            SelectDetectionAreaButton.BackColor = Color.DarkGray;
            SelectDetectionAreaButton.Enabled = false;

            Refresh(sender, e);

            if (selectionForm == null || selectionForm.IsDisposed)
            {
                selectionForm = new Form();
                selectionForm.FormBorderStyle = FormBorderStyle.None;
                selectionForm.MouseDown += SelectionFormMouseDown;
                selectionForm.MouseMove += SelectionFormMouseMove;
                selectionForm.MouseUp += SelectionFormMouseUp;
                selectionForm.KeyDown += SelectionFormKeyDown;
                selectionForm.Paint += SelectionFormPaint;

                Bitmap captureBitmap = SelectionBitmap();
                StartButton.BackColor = Color.DarkGray;
                StartButton.Enabled = false;
                SelectedAreaPicturebox.Image = captureBitmap;
                selectionForm.BackgroundImage = captureBitmap;
                selectionForm.Size = captureBitmap.Size;
                selectionForm.FormClosed += SelectionFormFormClosed;
            }
            else
            {
                selectionForm.BringToFront();
            }

            selectionForm.Show();
        }

        //Sound buttons
        private void TestKillSoundButtonClick(object sender, EventArgs e)
        {
            PlaySound(KillSoundPath, KillVolumeSlider.Value);
        }
        private void TestDeathSoundButtonClick(object sender, EventArgs e)
        {
            PlaySound(DeathSoundPath, DeathVolumeSlider.Value);
        }
        private void TestAssistSoundButtonClick(object sender, EventArgs e)
        {
            PlaySound(AssistSoundPath, AssistVolumeSlider.Value);
        }

        //Sound selection buttons
        private void SelectKillSoundButtonClick(object sender, EventArgs e)
        {
            KillSoundPath = GetSoundPath();
            if (KillSoundPath != null)
            {
                TestKillSoundButton.Enabled = true;
                string filename = Path.GetFileName(KillSoundPath);
                if (filename.Length > 30)
                {
                    filename = filename.Substring(0, 30) + "...";
                }
                KillSoundLabel.Text = filename;
            }
        }
        private void SelectDeathSoundButtonClick(object sender, EventArgs e)
        {
            DeathSoundPath = GetSoundPath();
            if (DeathSoundPath != null)
            {
                TestDeathSoundButton.Enabled = true;

                string filename = Path.GetFileName(DeathSoundPath);
                if (filename.Length > 30)
                {
                    filename = filename.Substring(0, 30) + "...";
                }
                DeathSoundLabel.Text = filename;
            }
        }
        private void SelectAssistSoundButtonClick(object sender, EventArgs e)
        {
            AssistSoundPath = GetSoundPath();
            if (AssistSoundPath != null)
            {
                TestAssistSoundButton.Enabled = true;
                string filename = Path.GetFileName(AssistSoundPath);
                if (filename.Length > 30)
                {
                    filename = filename.Substring(0, 30) + "...";
                }
                AssistSoundLabel.Text = filename;
            }
        }

        //Links
        private void Zylop6YouTube_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.youtube.com/@Zylop6");
        }
        private void Zylop6Instagram_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.instagram.com/zylop6");
        }
        private void Zylop6GitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Zylop6");
        }
        private void Zylop6Twitch_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.twitch.tv/zylop6");
        }

        //==========================================================================================================
        //-------------- Miscellaneous  --------------
        //Scanning interval
        private void ScanIntervalChanged(object sender, EventArgs e)
        {
            if (ScanIntervalScale.Value < 250)
            {
                ScanIntervalScaleLabel.Text = ScanIntervalScale.Value + " ms";
            }
            if (ScanIntervalScale.Value > 250)
            {
                ScanIntervalScaleLabel.Text = Math.Round((ScanIntervalScale.Value / 1000f),2).ToString() + " s";
            }
        }

        //Display  volumes
        private void KillVolumeChanged(object sender, EventArgs e)
        {
            KillVolumeLabel.Text = KillVolumeSlider.Value + " %";
        }
        private void DeathVolumeChanged(object sender, EventArgs e)
        {
            DeathVolumeLabel.Text = DeathVolumeSlider.Value + " %";
        }
        private void AssistVolumeChanged(object sender, EventArgs e)
        {
            AssistVolumeLabel.Text = AssistVolumeSlider.Value + " %";
        }

        //Image editing
        private Bitmap InvertImage(Image image)
        {
            Bitmap invertedImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = ((Bitmap)image).GetPixel(x, y);
                    Color invertedColor = Color.FromArgb(
                        255 - pixelColor.R,
                        255 - pixelColor.G,
                        255 - pixelColor.B
                    );

                    invertedImage.SetPixel(x, y, invertedColor);
                }
            }

            return invertedImage;
        }
        private Bitmap ToBlackWhite(Image image)
        {
            Bitmap blackWhiteImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = ((Bitmap)image).GetPixel(x, y);

                    // Calculate the grayscale value for the pixel
                    int grayscale = (int)(
                        pixelColor.R * 0.299 + pixelColor.G * 0.587 + pixelColor.B * 0.114
                    );

                    // Determine whether to set the pixel to black or white
                    Color newColor = (grayscale >= 24) ? Color.White : Color.Black;

                    blackWhiteImage.SetPixel(x, y, newColor);
                }
            }

            return blackWhiteImage;
        }
        private Bitmap ToGrayScale(Image image)
        {
            Bitmap grayScaleImage = new Bitmap(image.Width, image.Height);

            using (Graphics graphics = Graphics.FromImage(grayScaleImage))
            {
                // Create a color matrix to convert the image to grayscale
                ColorMatrix colorMatrix = new ColorMatrix(
                    new float[][]
                    {
                        new float[] { 0.9f, 0.9f, 0.9f, 0, 0 },
                        new float[] { 0.9f, 0.9f, 0.9f, 0, 0 },
                        new float[] { 0.9f, 0.9f, 0.9f, 0, 0 },
                        new float[] { 0, 0, 0, 1, 0 },
                        new float[] { 0, 0, 0, 0, 1 }
                    }
                );

                // Create an ImageAttributes object and set the color matrix
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(colorMatrix);

                    // Draw the original image onto the grayscale image
                    graphics.DrawImage(
                        image,
                        new Rectangle(0, 0, image.Width, image.Height),
                        0,
                        0,
                        image.Width,
                        image.Height,
                        GraphicsUnit.Pixel,
                        attributes
                    );
                }
            }

            return grayScaleImage;
        }

        //Sounds
        private void SoundPlayer()
        {
            if (KillCount > OldKillCount)
            {
                PlaySound(KillSoundPath, KillVolumeSlider.Value);
            }
            if (DeathCount > OldDeathCount)
            {
                PlaySound(DeathSoundPath, DeathVolumeSlider.Value);
            }
            if (AssistCount > OldAssistCount)
            {
                PlaySound(AssistSoundPath, AssistVolumeSlider.Value);
            }

            OldKillCount = KillCount;
            OldDeathCount = DeathCount;
            OldAssistCount = AssistCount;

        }
        public string GetSoundPath()
        {
            string Sound = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Sound Files (*.wav, *.mp3)|*.wav;*.mp3";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Sound = openFileDialog.FileName;
            }
            return Sound;
        }
        private void PlaySound(string soundPath, int volume)
        {
            if (MuteAllCheckBox.Checked == false)
            {
                if (soundPath != null)
                {
                    mediaPlayer.URL = soundPath;
                    mediaPlayer.settings.volume = volume * 2;
                    mediaPlayer.controls.play();
                }
            }
        }

        //==========================================================================================================
        //-------------- Forms  --------------
        //Selection form
        private void SelectionFormFormClosed(object sender, FormClosedEventArgs e)
        {
            if (SelectionBitmap() != null)
            {
                SelectDetectionAreaButton.BackColor = Color.FromArgb(125, 102, 204);
                SelectDetectionAreaButton.Enabled = true;
                StartButton.Enabled = true;
                StartButton.BackColor = Color.FromArgb(15, 250, 150);

                if (selectionRectangle != null)
                {
                    areaSelected = true;
                    UpdatePreviewPictureBox();
                }
            }
        }
        private void SelectionFormMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                initialPosition = e.Location;
                isDrawingRectangle = true;
            }
        }
        private void SelectionFormMouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawingRectangle)
            {
                int width = Math.Abs(e.Location.X - initialPosition.X);
                int height = Math.Abs(e.Location.Y - initialPosition.Y);
                int x = Math.Min(e.Location.X, initialPosition.X);
                int y = Math.Min(e.Location.Y, initialPosition.Y);
                selectionRectangle = new Rectangle(x, y, width, height);
                selectionForm.Invalidate();
            }
        }
        private void SelectionFormMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawingRectangle = false;
            }
        }
        private void SelectionFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectionForm.Close();

                SelectDetectionAreaButton.BackColor = Color.FromArgb(125, 102, 204);
                SelectDetectionAreaButton.Enabled = true;
                StartButton.Enabled = true;
                StartButton.BackColor = Color.FromArgb(15, 250, 150);
            }
        }
        private void SelectionFormPaint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawRectangle(pen, selectionRectangle);
            }
        }
    }
}
