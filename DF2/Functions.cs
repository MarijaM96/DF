using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Security.Cryptography;
using System.IO;
using static DF2.TextHuffman;
using static DF2.Tables;

namespace DF2
{
   public class Functions
    {
        #region Variables

        public static float[,] yarray, cbarray, crarray, yarraybig, cbarraybig, crarraybig;
        public static float[,] yoriginal, cboriginal, croriginal;
        public static int bwidth, bheight, bwidthnew = 0, bheightnew = 0;
        public static List<float[,]> blocksY, blocksCb, blocksCr;
        public static List<int[,]> quantizedY, quantizedCb, quantizedCr;

        public static Dictionary<int, int> freqImg, freqImgRet;
        public static List<bool> compressedImage, compressedImageRet;
        public static List<int> DCcoefY, DCcoefCb, DCcoefCr, DCcoefYRet, DCcoefCbRet, DCcoefCrRet;

        public static byte[] key, iv;
        public static Dictionary<char, int> freq; 
        public static List<bool> cryptedcompressed;

        public static String msgret="";
        public static int ctr;
        public static int mask = 2147483646; //1111...1110

        #endregion

        public static Bitmap ApplySteganography(Bitmap coverimage, String hiddentext)
        {
            RGBtoYCbCr(coverimage);
            FillChannels();
            ChannelsToBlocks();
            DCTAll();
            Quantize();
            EmbedMessage(cryptedcompressed);
            return ShowPicture(); 
        }

        public static Bitmap ShowPicture()
        {
            msgret = DecryptAndDecompressMessage(ExtractMessage(), freq, MainForm.aes);
            Dequantize();
            IDCTAll();
            BlocksToChannels();
            RetOriginalChannels();
            return YCbCrtoRGB(); 
        }

        #region ColorConversion 
        
        public static void RGBtoYCbCr (Bitmap m)
        {
            Bitmap n = new Bitmap(m);
            yarray = new float[bwidth, bheight];
            cbarray = new float[bwidth, bheight];
            crarray = new float[bwidth, bheight];

            for(int i=0;i<bwidth;i++)
                for(int j=0;j<bheight;j++)
                {
                    Color c = m.GetPixel(i, j);
                    yarray[i,j] = (float)(0.299 * c.R + 0.587 * c.G + 0.114 * c.B);
                    cbarray[i, j] = (float)(128 - (0.168736 * c.R) - (0.331264 * c.G) + 0.5 * c.B);
                    crarray[i, j] = (float)(128 + 0.5* c.R - (0.418688 * c.G) - (0.081312 * c.B));
                }
        }

        public static Bitmap YCbCrtoRGB()
        {
            Bitmap n = new Bitmap(bwidth, bheight);
            byte r, g, b;
            for (int i = 0; i < bwidth; i++)
                for (int j = 0; j < bheight; j++)
                {
                    r = (byte)(yoriginal[i, j] + 1.402 * (croriginal[i, j] - 128));
                    g = (byte)(yoriginal[i, j] - (0.344136 * (cboriginal[i, j] - 128)) - (0.714136 * (croriginal[i, j] - 128)));
                    b = (byte)(yoriginal[i, j] + 1.772 * (cboriginal[i, j] - 128));

                    n.SetPixel(i, j, System.Drawing.Color.FromArgb(r, g, b));
                }
            return n;
        }
        #endregion

        #region 8*8Blocks

        public static void ChannelsToBlocks()
        {
            int indY = 0;
            blocksY = new List<float[,]>();
            blocksCb = new List<float[,]>();
            blocksCr = new List<float[,]>();
            
            int ctrw, ctrh, x, y, a, b;
            int numwidth = bwidthnew / 8;
            int numheight = bheightnew / 8;

            ctrw = 0;
            while(ctrw<numwidth)
            {
                ctrh = 0; a = 0; b = 0;
                while(ctrh<numheight)
                {
                    float[,] tmpmaty = new float[8, 8];
                    float[,] tmpmatcb = new float[8, 8];
                    float[,] tmpmatcr = new float[8, 8];
                    a = 0;
                    for(x=ctrw*8; x< ctrw*8+8;x++)
                    {
                        b = 0;
                        for(y=ctrh*8;y<ctrh*8+8;y++)
                        {
                            tmpmaty[a, b] = yarraybig[x, y];
                            tmpmatcb[a, b] = cbarraybig[x, y];
                            tmpmatcr[a, b] = crarraybig[x, y];
                            b++;
                        }
                        a++;
                    }
                    blocksY.Insert(indY++, tmpmaty);
                    blocksCb.Add(tmpmatcb);
                    blocksCr.Add(tmpmatcr);
                    ctrh++;
                }
                ctrw++;
            }
        }

        public static void BlocksToChannels()
        {
            int ctrw, ctrh, x, y, a, b;
            int numwidth = bwidthnew / 8;
            int numheight = bheightnew / 8;
            ctrw = 0;
            while (ctrw < numwidth)
            {
                float[,] tmpy = new float[8, 8];
                float[,] tmpcb = new float[8, 8];
                float[,] tmpcr = new float[8, 8];
                ctrh = 0; a = 0; b = 0;
                
                while (ctrh < numheight)
                {
                    tmpy = blocksY [ctrw * numheight + ctrh];
                    tmpcb = blocksCb[ctrw * numheight + ctrh];
                    tmpcr = blocksCr[ctrw * numheight + ctrh];
                    a = 0;
                    for (x = ctrw * 8; x < ctrw * 8 + 8; x++)
                    {
                        b = 0;
                        for (y = ctrh * 8; y < ctrh * 8 + 8; y++)
                        {
                            yarraybig[x, y] = tmpy[a, b];
                            cbarraybig[x, y] = tmpcb[a, b];
                            crarraybig[x, y] = tmpcr[a, b];
                            b++;
                        }
                        a++;
                    }
                    ctrh++;
                }
                ctrw++;
            }

        }

        public static void FillChannels()
        {
            CheckDimensions(bwidth, bheight, out bwidthnew, out bheightnew);
            yarraybig = new float[bwidthnew, bheightnew];
            cbarraybig = new float[bwidthnew, bheightnew];
            crarraybig = new float[bwidthnew, bheightnew];
            int i, j;
            if (bheightnew > bheight)
            {
                for (i = 0; i < bwidth; i++)
                    for (j = bheight; j < bheightnew; j++)
                    {
                        yarraybig[i, j] = yarray[i, bheight - 1];
                        cbarraybig[i, j] = cbarray[i, bheight - 1];
                        crarraybig[i, j] = crarray[i, bheight - 1];
                    }
            }
            if (bwidthnew > bwidth)
            {
                for (i = bwidth; i < bwidthnew; i++)
                    for (j = 0; j < bheight; j++)
                    {
                        yarraybig[i, j] = yarray[bwidth - 1, j];
                        cbarraybig[i, j] = cbarray[bwidth - 1, j];
                        crarraybig[i, j] = crarray[bwidth - 1, j];
                    }

            }
            if (bheightnew > bheight && bwidthnew > bwidth)
            {
                for (i = bwidth; i < bwidthnew; i++)
                    for (j = bheight; j < bheightnew; j++)
                    {
                        yarraybig[i, j] = yarraybig[bwidth - 1, j];
                        cbarraybig[i, j] = cbarraybig[bwidth - 1, j];
                        crarraybig[i, j] = crarraybig[bwidth - 1, j];
                    }
            }
            for (i = 0; i < bwidth; i++)
                for (j = 0; j < bheight; j++)
                {
                    yarraybig[i, j] = yarray[i, j];
                    cbarraybig[i, j] = cbarray[i, j];
                    crarraybig[i, j] = crarray[i, j];
                }
        } 

        public static void RetOriginalChannels()
        {
            yoriginal = new float[bwidth, bheight];
            cboriginal = new float[bwidth, bheight];
            croriginal = new float[bwidth, bheight];

            for(int i=0;i<bwidth;i++)
                for(int j=0; j<bheight;j++)
                {
                    yoriginal[i, j] = yarraybig[i, j];
                    cboriginal[i, j] = cbarraybig[i, j];
                    croriginal[i, j] = crarraybig[i, j];
                }
        }

        #endregion

        #region DCT/IDCT

        public static void DCTAll()
        {
            int num = bwidthnew * bheightnew / 64;
            for (int i = 0; i < num; i++)
            {
                Subtract128(blocksY[i]);
                Subtract128(blocksCb[i]);
                Subtract128(blocksCr[i]);

                DCTBlock(blocksY[i]);
                DCTBlock(blocksCb[i]);
                DCTBlock(blocksCr[i]);
            }

        }

        public static void IDCTAll()
        {
            int num = bwidthnew * bheightnew / 64;
            for (int i = 0; i < num; i++)
            {
                IDCTBlock(blocksY[i]);
                IDCTBlock(blocksCb[i]);
                IDCTBlock(blocksCr[i]);

                Add128(blocksY[i]);
                Add128(blocksCb[i]);
                Add128(blocksCr[i]);
            }
        }

        public static void DCTBlock(float[,] m)
        {
            int k1, k2, i, j;
            float[,] B = new float[8, 8];

            for (k1 = 0; k1 < 8; k1++)
                for (k2 = 0; k2 < 8; k2++)
                {
                    B[k1, k2] = 0;
                    for (i = 0; i < 8; i++) 
                        for (j = 0; j < 8; j++) 
                            B[k1, k2] += (float)(m[i, j] * CalcCos(k1, i) * CalcCos(k2, j));  
                    B[k1, k2] *= (float)(CalcNormFunc(k1) * CalcNormFunc(k2)*0.25); 
                }
            for (i = 0; i < 8; i++)
                for (j = 0; j < 8; j++)
                    m[i, j] = B[i, j];
        }

        public static void IDCTBlock(float[,] m)
        {
            int k1, k2, i, j;
            float[,] B = new float[8, 8];

            for (k1 = 0; k1 < 8; k1++)
                for (k2 = 0; k2 < 8; k2++)
                {
                    B[k1, k2] = 0;
                    for (i = 0; i < 8; i++)
                        for (j = 0; j < 8; j++)
                            B[k1, k2] += (float)(m[i, j] * CalcCos(i, k1) * CalcCos(j, k2) * CalcNormFunc(i) * CalcNormFunc(j));

                    B[k1, k2] /= (float)(4.0);
                }
            for (i = 0; i < 8; i++)
                for (j = 0; j < 8; j++)
                    m[i, j] = B[i, j];
        }

        #endregion

        #region Quantization/Dequantization 

        public static void Quantize()
        {
            quantizedY = new List<int[,]>();
            quantizedCb = new List<int[,]>();
            quantizedCr = new List<int[,]>();
            for (int i=0;i<bwidthnew*bheightnew/64;i++)
            {
                int[,] quantizedblockY = new int[8, 8];
                quantizedblockY= QuantizeBlock(blocksY[i], Standard_Luminance_Quantization_Table);
                quantizedY.Add(quantizedblockY);
                int[,] quantizedblockCb = new int[8, 8];
                quantizedblockCb = QuantizeBlock(blocksCb[i], Standard_Chrominance_Quantization_Table);
                quantizedCb.Add(quantizedblockCb);
                int[,] quantizedblockCr = new int[8, 8];
                quantizedblockCr = QuantizeBlock(blocksCr[i], Standard_Chrominance_Quantization_Table);
                quantizedCr.Add(quantizedblockCr);
            }
        }

        public static void Dequantize()
        {
            for (int i = 0; i < bwidthnew * bheightnew / 64; i++)
            {
                blocksY[i] = DequantizeBlock(quantizedY[i], Standard_Luminance_Quantization_Table);
                blocksCb[i] = DequantizeBlock(quantizedCb[i], Standard_Chrominance_Quantization_Table);
                blocksCr[i] = DequantizeBlock(quantizedCr[i], Standard_Chrominance_Quantization_Table);
            }
        }

        public static int[,] QuantizeBlock(float[,] dctcoefblock, int[] table)
        {
            int i, j;
            int[,] S1 = new int[8, 8];

            for (i = 0; i < 8; i++)
                for (j = 0; j < 8; j++)
                {
                   S1[i, j] = (int)(dctcoefblock[i, j] / (float)table[i * 8 + j]);
                }
            return S1;
        }

        public static float[,] DequantizeBlock(int[,] quantizedblock, int[] table)
        {
            int i, j;
            float[,] S1 = new float[8, 8];

            for (i = 0; i < 8; i++)
                for (j = 0; j < 8; j++)
                {
                    S1[i, j] = (float)(quantizedblock[i, j] * table[i * 8 + j]);
                }
            return S1;
        }
      
        #endregion

        #region MessageEmbedding/MessageExtracting

        public static void EmbedMessage(List<bool> msg)
        {
            EmbedLength(msg.Count);
            int p = msg.Count;
            int ptr = 0;
            while (ptr != p)
            {
                for (int i = 4; i < quantizedCb.Count; i++)
                {
                    if((ptr+4)<p)
                     ptr = EmbedInBlock(quantizedCb[i], msg, ptr);

                    if ((ptr + 4) < p)
                        ptr = EmbedInBlock(quantizedCr[i], msg, ptr);

                    if (ptr == p)  break;

                    ptr = EmbedOne(quantizedY[i - 4], msg, ptr);
                    if (ptr == p) break;
                }
            }

        }

        public static int EmbedInBlock(int[,] block, List<bool> msg, int ptr)
        {
            int[] bits = new int[4];
            for (int m = 0; m < 4; m++)
            {
                bits[m]= (msg[ptr + m]) ? 1 : 0;
            }

            block[0, 1] = (block[0, 1] & mask) ^ bits[0];
            block[0, 2] = (block[0, 2] & mask) ^ bits[1];
            block[1, 0] = (block[1, 0] & mask) ^ bits[2];
            block[2, 0] = (block[2, 0] & mask) ^ bits[3];

            return ptr+4;
        }

        public static int EmbedOne(int[,] block, List<bool> msg, int ptr)
        {
            int bit = (msg[ptr])? 1 : 0;
            block[0, 2] = (block[0, 2] & mask) ^ bit;
            return ptr + 1;
        }

        public static void EmbedLength(int length)
        {
            int shhigher = (length >> 16);
            int shlower = length & 65535;
            EmbedLengthInChannel(quantizedCb, shhigher);
            EmbedLengthInChannel(quantizedCr, shlower);
        }

        public static void EmbedLengthInChannel(List<int[,]> channel, int len)
        {
            for(int x=0;x<4;x++)
            {
                int i = 0, j = 1;
                int l = (len >> (15 - x * 4)) & 1;
                channel[x][i, j] = (channel[x][i, j] & mask) ^ l;
                l = (len >> (14 - x * 4)) & 1;
                channel[x][j, i] = (channel[x][j, i] & mask) ^ l;
                j++;
                l = (len >> (13 - x * 4)) & 1;
                channel[x][i, j] = (channel[x][i, j] & mask) ^ l;
                l = (len >> (12 - x * 4)) & 1;
                channel[x][j, i] = (channel[x][j, i] & mask) ^ l;
            }
        }

        public static List<bool> ExtractMessage()
        {
            int msglen = ExtractLength();
            List<bool> ret = new List<bool>();
            int ptr = 0;
            while (ptr != msglen)
            {
                for (int i = 4; i < quantizedCb.Count; i++)
                {
                    if ((ptr + 4) < msglen)
                    {
                        ret.AddRange(ExtractFromBlock(quantizedCb[i]));
                        ptr += 4;
                    }

                    if ((ptr + 4) < msglen)
                    {
                        ret.AddRange(ExtractFromBlock(quantizedCr[i]));
                        ptr += 4;
                    }
                        
                    if (ptr == msglen)  break;

                    ret.Add(ExtractOne(quantizedY[i-4]));
                    ptr++;
                    if (ptr == msglen) break;
                }
            }
            return ret;
        }

        public static List<bool> ExtractFromBlock(int[,] block)
        {
            List<bool> ret = new List<bool>();
            int bit = block[0, 1] & 1;
            ret.Add((bit == 1) ? true : false);
            bit = block[0, 2] & 1;
            ret.Add((bit == 1) ? true : false);
            bit = block[1, 0] & 1;
            ret.Add((bit == 1) ? true : false);
            bit = block[2, 0] & 1;
            ret.Add((bit == 1) ? true : false);
            return ret;
        }

        public static bool ExtractOne(int[,] block)
        {
            return ((block[0, 2] & 1) == 1) ? true: false;
        }

        public static int ExtractLength()
        {
            int len = 0;
            int higher = ExtractLengthFromChannel(quantizedCb);
            int lower = ExtractLengthFromChannel(quantizedCr);
            len = (higher << 16) | lower;
            return len;
        }

        public static int ExtractLengthFromChannel(List<int[,]> channel)
        {
            int l = 0, res = 0;
            for (int x = 0; x < 4; x++)
            {
                int i = 0, j = 1;
                res = (channel[x][i,j] & 1)<<(15-x*4);
                l = l | res;
                res = (channel[x][j, i] & 1) << (14 - x * 4);
                l = l | res;
                j++;
                res = (channel[x][i, j] & 1) << (13 - x * 4);
                l = l | res;
                res = (channel[x][j, i] & 1) << (12 - x * 4);
                l = l | res;
            }
            return l;
        }
        
        #endregion

        #region ImageCompression/Decompression

        public static Dictionary<int, int> Compress(List<int> c, out List<bool> a)
        {
            List<bool> msg = new List<bool>();
            ImageHuffman.HuffmanTree ht = new ImageHuffman.HuffmanTree();
            c.Add(1001); //granicnik
            ht.MakeTree(c);

            List<bool> encoded = ht.CompressImage(c);
            foreach (bool b in encoded)
                msg.Add(b);
            a = msg;
            return ht.freq;
        }

        public static List<int> Decompress(List<bool> a, Dictionary<int, int> frequencies)
        {
           ImageHuffman.HuffmanTree ht = new ImageHuffman.HuffmanTree();
            ht.freq = frequencies;
            ht.MakeTreeWithFrequencies(); 
            List<int> tmp = ht.DecompressImage(a);
            tmp.Remove(1001); //granicnik
            return tmp;
        }

        public static List<int> GatherChannels()
        {
            List<int> listaY = IntMatrixToIntArrayListChannel(quantizedY, out DCcoefY);
            List<int> listaCb = IntMatrixToIntArrayListChannel(quantizedCb, out DCcoefCb);
            List<int> listaCr = IntMatrixToIntArrayListChannel(quantizedCr, out DCcoefCr);
            List<int> listAll = new List<int>();
            listAll.AddRange(listaY);
            listAll.AddRange(listaCb);
            listAll.AddRange(listaCr);
            return listAll;
        }

        public static void SeparateChannels(List<int> a)
        {
            int len = 63 * bwidthnew * bheightnew / 64;
            List<int> Ypiece = a.GetRange(0, len);
            List<int> Cbpiece = a.GetRange(len, len);
            List<int> Crpiece = a.GetRange(2 * len, len);
            quantizedY = IntArrayListToIntMatrixChannel(Ypiece, DCcoefYRet);
            quantizedCb = IntArrayListToIntMatrixChannel(Cbpiece, DCcoefCbRet);
            quantizedCr = IntArrayListToIntMatrixChannel(Crpiece, DCcoefCrRet);
        }

        public static List<int> IntMatrixToIntArrayListChannel(List<int[,]> a, out List<int> dc)
        {
            dc = new List<int>();
            List<int> vracanje = new List<int>();
            for (int i = 0; i < bwidthnew * bheightnew / 64; i++)
            {
                int[,] tmp = a[i];
                for (int j = 0; j < 8; j++)
                    for (int k = 0; k < 8; k++)
                    {
                        if (j == 0 && k == 0)
                            dc.Add(tmp[j, k]);
                        else
                            vracanje.Add(tmp[j, k]);
                    }
            }
            return vracanje;
        }

        public static List<int[,]> IntArrayListToIntMatrixChannel(List<int> a, List<int> dc)
        {
            List<int[,]> vracanje = new List<int[,]>();
            int ctr = 0;
            for (int i = 0; i < bwidthnew * bheightnew / 64; i++)
            {
                int[,] tmp = new int[8, 8];
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        if (k == 0 && j == 0)
                        {
                            tmp[j, k] = dc[ctr++];
                        }
                        else
                            tmp[j, k] = a[i * 63 + j * 8 + k - 1];
                    }
                }
                vracanje.Add(tmp);
            }
            return vracanje;
        }
        #endregion

        #region Save/LoadFormat

        public static void SaveFormat(string filename)
        {
            freqImg = Compress(GatherChannels(), out compressedImage);
            int[] basicDimensions = new int[3];
            int numberOfBytes = compressedImage.Count;
            List<int> dccoefss;
            int padding = 0;

            basicDimensions[0] = bwidth;
            basicDimensions[1] = bheight;
            basicDimensions[2] = compressedImage.Count;
            ListOfDCs(out dccoefss);
     
            List<int> keyvaluekeyvalue = new List<int>(freqImg.Count * 2);
            foreach (KeyValuePair<int, int> kvp in freqImg)
            {
                keyvaluekeyvalue.Add(kvp.Key);
                keyvaluekeyvalue.Add(kvp.Value);
            }

            byte[] newImgData = BoolsToBytes(compressedImage, out padding);

            BinaryWriter bw;
            try
            {
                bw = new BinaryWriter(new FileStream(filename, FileMode.Create));
                for (int i = 0; i < 3; i++)
                {
                    bw.Write(basicDimensions[i]);
                }
                for(int i=0;i<dccoefss.Count;i++)
                {
                    bw.Write(dccoefss[i]);
                }

                bw.Write(freqImg.Count);
                for(int i=0;i<2* freqImg.Count;i++)
                {
                   bw.Write(keyvaluekeyvalue[i]);
                }

                bw.Write(padding);
                bw.Write(newImgData);

                bw.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        public static void LoadFormat(string filename)
        {
            int padding, freqimgcount;
            int numofdc, listofintdcs;
            List<int> dccoefss;
            int compressedimagelengthinbools;
            byte[] newImgData;

            BinaryReader br; 
            try
            {
                br = new BinaryReader(new FileStream(filename, FileMode.Open));
                bwidth = br.ReadInt32();
                bheight = br.ReadInt32();
                compressedimagelengthinbools = br.ReadInt32();
                int comprImLenInBytes = compressedimagelengthinbools / 8;
                CheckDimensions(bwidth, bheight, out bwidthnew, out bheightnew);
                numofdc = bwidthnew * bheightnew / 64;
                listofintdcs = 3 * numofdc;
                dccoefss = new List<int>(listofintdcs);
                for (int i = 0; i < listofintdcs; i++)
                    dccoefss.Add(br.ReadInt32());

                freqimgcount = br.ReadInt32();
                freqImgRet = new Dictionary<int, int>();
                for(int i=0;i<freqimgcount;i++)
                    freqImgRet.Add(br.ReadInt32(), br.ReadInt32()); 

                padding = br.ReadInt32();
                if(padding!=0)
                    newImgData = new byte[comprImLenInBytes + 1];
                else
                    newImgData = new byte[comprImLenInBytes];

                for (int i = 0; i < comprImLenInBytes; i++)
                    newImgData[i] = br.ReadByte();

                if (padding != 0)
                    newImgData[comprImLenInBytes] = br.ReadByte();

                br.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            compressedImageRet = new List<bool>();
            compressedImageRet = BytesToBool(newImgData, padding);
            MakeThreeDCLists(dccoefss);
        }

        public static void ListOfDCs(out List<int> all)
        {
            all = new List<int>();
            all.AddRange(DCcoefY);
            all.AddRange(DCcoefCb);
            all.AddRange(DCcoefCr);
        }
        
        public static void MakeThreeDCLists(List<int> dcs)
        {
            int i;
            DCcoefYRet = new List<int>();
            DCcoefCbRet = new List<int>();
            DCcoefCrRet = new List<int>();
            for (i = 0; i < dcs.Count() / 3; i++)
                DCcoefYRet.Add(dcs[i]);
            for (i = dcs.Count() / 3; i < 2 * dcs.Count() / 3; i++)
                DCcoefCbRet.Add(dcs[i]);
            for(i = 2* dcs.Count() / 3; i < dcs.Count(); i++)
                DCcoefCrRet.Add(dcs[i]);
        }

        public static byte[] BoolsToBytes(List<bool> compr, out int dopuna)
        {
            byte[] niz;
            int tmp, i, j;
            int div = compr.Count / 8;
            int mod = compr.Count % 8;
            dopuna = 8 - mod;
            if (dopuna != 0)
                niz = new byte[div + 1];
            else
                niz = new byte[div];

            for(i=0;i< div; i++)
            {
                niz[i] = 0;
                for(j=0;j<8;j++)
                {
                    if (compr[i * 8 + j])
                        tmp = 1;
                    else
                        tmp = 0;
                    niz[i] = (byte)((niz[i] << 1) ^ tmp);
                }
            }
            
            if(dopuna!=0)
            {
                for(j=0;j< mod; j++)
                {
                    if (compr[div*8 + j])
                        tmp = 1;
                    else
                        tmp = 0;
                    niz[div] = (byte)((niz[div] << 1) ^ tmp);
                }
                niz[div] =(byte)( niz[div] << dopuna);
            }
            return niz;
        }

        public static List<bool> BytesToBool(byte[] niz, int dopuna)
        {
            List<bool> compressedImage = new List<bool>();
            int dim = niz.Count();
            for (int i=0;i<dim-1;i++)
            {
                byte tmp = niz[i];
                for(int j=0;j<8;j++)
                {
                    int pom = (tmp >> (7-j)) & 1;
                    if (pom == 0)
                        compressedImage.Add(false);
                    else
                        compressedImage.Add(true);
                }
            }
            if(dopuna!=0)
            {
                byte tmp = niz[dim - 1];
                for(int j=0; j<8-dopuna;j++)
                {
                    int pom = (tmp >> (7 - j)) & 1;
                    if (pom == 0)
                        compressedImage.Add(false);
                    else
                        compressedImage.Add(true);
                }
            }
            return compressedImage;
        }

        #endregion

        #region HiddenTextEncryptionAndCompression

        public static void EncryptAndCompressMessage(String hiddentext, out byte[] key, out byte[] iv, bool aes)
        {
            if (aes)
            {
                String encryptedText = Functions.EncryptWithAES(hiddentext, out key, out iv);
                freq = CompressTextHuffman(encryptedText, out cryptedcompressed);
            }
            else
            {
                freq = CompressTextHuffman(hiddentext, out cryptedcompressed);
                key = new byte[1]; iv = new byte[1];
            }
        }

        public static String DecryptAndDecompressMessage(List<bool> crcomp, Dictionary<char, int> f, bool aes)
        {
            String decompressed = DecompressTextHuffman(crcomp, f);
            if (aes)
                return DecryptWithAES(decompressed, key, iv);
            else
                return decompressed;
        }

        public static Dictionary<char, int> CompressTextHuffman(string txt, out List<bool> a)
        {
            List<bool> msg = new List<bool>();
            HuffmanTree ht = new HuffmanTree();
            txt += '\0';
            ht.MakeTree(txt);

            List<bool> encoded = ht.CompressText(txt);
            foreach (bool b in encoded)
              msg.Add(b);
            a = msg;
            return ht.freq;
        }

        public static string DecompressTextHuffman (List<bool> a, Dictionary<char, int> f)
        {
            HuffmanTree ht = new HuffmanTree();
            ht.freq = f;
            ht.MakeTreeWithFrequencies();
            return ht.DecompressText(a).Trim('\0');
        }

        public static byte[] EncryptString(string txt, byte[] Key, byte[] IV)
        {
            if (txt == null || txt.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key; aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(txt);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }

        public static string EncryptWithAES(string hiddentext, out byte[] key, out byte[] iv)
        {
            Aes myAes = Aes.Create();
            iv = myAes.IV;
            key = myAes.Key;

            byte[] encrypted = EncryptString(hiddentext, myAes.Key, myAes.IV);
            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptString(byte[] ciphertxt, byte[] Key, byte[] IV)
        {
            if (ciphertxt == null || ciphertxt.Length <= 0)
                throw new ArgumentNullException("cipher Text");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key; aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(ciphertxt))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

        public static string DecryptWithAES(string decrypt, byte[] key, byte[] iv)
        {
            Aes myAes = Aes.Create();
            myAes.IV = iv;
            myAes.Key = key;

            byte[] bytes = Convert.FromBase64String(decrypt);
            string decrypted = DecryptString(bytes, key, iv);
            return decrypted;
        }

        #endregion

        #region MinorFunctions

        public static void CheckDimensions(int w, int h, out int wnew, out int hnew)
        {
           wnew = (w % 8 != 0) ? w + 8 - w % 8 : w;
           hnew = (h % 8 != 0) ? h + 8 - h % 8 : h;
        }

        public static void Subtract128(float[,] block)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    block[i, j] -=128;
        }

        public static void Add128(float[,] block)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    block[i, j] += 128;
        }

        public static double CalcCos(int a, int b)
        {
            return Math.Cos(Math.PI * a * (2 * b + 1) / 16);
        }

        public static double CalcNormFunc(int a)
        {
            return (a == 0) ? (1 / Math.Sqrt(2.0)) : 1.0;
        }

        #endregion

    }
}
