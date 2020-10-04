using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DF2.Functions;

namespace DF2
{
    public partial class MainForm : Form
    {
        public System.Drawing.Bitmap m_Bitmap;
        public static bool aes = false;

        public MainForm()
        {
            InitializeComponent();
            m_Bitmap = null;
            ctr = 0;
        }

        private void uploadCoverImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Application.StartupPath;
            openFileDialog.Filter = "Bitmap files (*.bmp)|*.bmp|Jpeg files (*.jpg)|*.jpg|GIF files(*.gif)|*.gif|PNG files(*.png)|*.png|All valid files|*.bmp/*.jpg/*.gif/*.png";
            openFileDialog.FilterIndex = 4;
            openFileDialog.RestoreDirectory = true;

            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                m_Bitmap = (Bitmap)Bitmap.FromFile(openFileDialog.FileName, false);
                pbxcover.Image = m_Bitmap;
                bwidth = m_Bitmap.Width;
                bheight = m_Bitmap.Height;
            }
        }

        private void hideWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_Bitmap == null)
                MessageBox.Show("Upload a cover image first.");
            else
            {
                ChooseObject co = new ChooseObject();
                if (DialogResult.OK == co.ShowDialog())
                {
                    if (!co.rbtnaeschecked && !co.rbtnplainchecked)
                    {
                        MessageBox.Show("Choose a way to hide your message.");
                    }
                    else
                    {
                        if (co.rbtnaeschecked)
                            aes = true;
                        if (!EnoughCapacity(co.hidetext, m_Bitmap.Width, m_Bitmap.Height, aes))
                        {
                            MessageBox.Show("Not enough capacity in this picture - shorten your message.");
                        }
                        else
                        {
                            var watch = new System.Diagnostics.Stopwatch();
                            watch.Start();
                            pbxstego.Image = ApplySteganography(m_Bitmap, co.hidetext);
                            watch.Stop();
                            MessageBox.Show($"Execution Time: {watch.ElapsedMilliseconds} ms");
                            saveToolStrip.Visible = true;
                        }
                    }
                }
            }
        }

        private void revealHiddenTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_Bitmap == null)
                MessageBox.Show("Upload a cover image first.");
            else
            {
                MessageBox.Show("Message: \n" + msgret);
            }
        }

        public static bool EnoughCapacity (String txt, int w, int h, bool aes)
        {
            EncryptAndCompressMessage(txt, out key, out iv, aes);
            if (Functions.cryptedcompressed.Count < (9*w * h / 64))
                return true;
            else
                return false;
        }

        private void saveToolStrip_Click(object sender, EventArgs e)
        {
            SaveFormat("stegoslika " + ctr + ".masa");
            MessageBox.Show("Saved.");
            pbxstego.Image = null;
            loadformatToolstrip.Visible = true;
        }

        private void loadformatToolstrip_Click(object sender, EventArgs e)
        {
            LoadFormat("stegoslika " + ctr + ".masa");
            ctr++;
            List<int> proba = Decompress(compressedImageRet, freqImgRet);
            SeparateChannels(proba);
            pbxstego.Image = ShowPicture();
            saveToolStrip.Visible = false;
            loadformatToolstrip.Visible = false;
        }
    }
}
