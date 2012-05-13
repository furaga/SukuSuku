using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SukuSuku
{
    class TemplateMatching
    {


        void CrossCorr()
        {

        }


        /// <summary>
        /// 積分画像を求める
        /// </summary>
        /// <param name="src">画像</param>
        /// <param name="sum">積分画像</param>
        /// <param name="sqsum">各ピクセル値を２乗した値に対する積分画像</param>
        /// <param name="w">画像幅</param>
        /// <param name="h">画像高</param>
        void Integral(byte[] src, double[] sum, double[] sqsum, int w, int h)
        {
            double s1, s2, s3;
            double sq1, sq2, sq3;
            int step = 3 * (w + 1);
            int srcIndex = 4 * (w + 1);
            int index = step;

            for (var i = 0; i < step; i++) sum[i] = sqsum[i] = 0;
            for (var i = step; i < sum.Length; i += step) sum[i] = sqsum[i] = 0;
            for (var y = 1; y < h + 1; y++)
            {
                s1 = sq1 = 0;
                s2 = sq2 = 0;
                s3 = sq3 = 0;
                srcIndex += 4;
                index += 3;
                for (var x = 1; x < w + 1; x++, srcIndex += 4)
                {
                    double it = src[srcIndex + 1];
                    s1 += it;
                    sq1 += it * it;
                    sum[index] = sum[index - step] + s1;
                    sqsum[index] = sqsum[index - step] + sq1;

                    index++;

                    it = src[srcIndex + 2];
                    s2 += it;
                    sq2 += it * it;
                    sum[index] = sum[index - step] + s2;
                    sqsum[index] = sqsum[index - step] + sq2;

                    index++;

                    it = src[srcIndex + 3];
                    s3 += it;
                    sq3 += it * it;
                    sum[index] = sum[index - step] + s3;
                    sqsum[index] = sqsum[index - step] + sq3;
                }
            }
        }

        /// <summary>
        /// 画像のピクセル値の平均・分散を求める
        /// </summary>
        /// <param name="src">画像</param>
        /// <param name="mean">各色の平均(mean.Length = 3)</param>
        /// <param name="variance">各色の分散(variance.Length = 3)</param>
        void MeanVariance(byte[] src, double[] mean, double[] variance)
        {
            mean[0] = variance[0] = mean[1] = variance[1] = mean[2] = variance[2] = 0;

            for (var i = 0; i < src.Length; i++)
            {
                var r = src[4 * i + 1];
                var g = src[4 * i + 2];
                var b = src[4 * i + 3];
                mean[0] += r;
                mean[1] += g;
                mean[2] += b;
                variance[0] += r * r;
                variance[1] += g * g;
                variance[2] += b * b;
            }

            var factor = 1.0 / (src.Length / 4);

            mean[0] *= factor;
            mean[1] *= factor;
            mean[2] *= factor;
            variance[0] = Math.Max(0.0, variance[0] * factor - mean[0] * mean[0]);
            variance[1] = Math.Max(0.0, variance[1] * factor - mean[1] * mean[1]);
            variance[2] = Math.Max(0.0, variance[2] * factor - mean[2] * mean[2]);
        }

        void matchTemplate()
        {

        }

        public void find()
        {

        }
    }
}
