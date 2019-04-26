using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QINLOCO
{
    class ManipulacaoDwgDxf
    {
        
        public void preencheDesenhoDxf(string nomeArquivo, string[] nomeNovo)
        {
            string[] nomeAntigo = new string[48] { "%fck", "%MT", "%MS", "%CAA", "%RAC1", "%RAC2", "%RAC3", "%RAC4", "%RAC5", "%RAC6", "%RAC7", "%RAC8", "%CMC1", "%CMC2", "%CMC3", "%CMC4", "%fck7", "%fck14", "%fck21", "%MT7", "%MT14", "%MT21", "%CB1", "%CB2", "%CB3", "%CB4", "%CB5", "%CB6", "%CB7", "%CB8", "%CB9", "%CB10", "%CB11", "%CB12", "%CB13", "%RCQ1", "%RCQ2", "PAVIMENTO %PAVIM", "%-.2CV", "%-.2CP", "%-.2CL", "%-.2CT", "%-.2HM", "%-.2FV", "%-.2FP", "%-.2FL", "%-.2FT", "%-.2AE" };
            
                for (int i = 0; i < 48; i++)
                {
                    insereInformacao(nomeArquivo, nomeAntigo[i], nomeNovo[i]);
                }   
         }

        private void insereInformacao(string nomeArquivo, string nomeAntigo, string nomeNovo)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(nomeArquivo, Encoding.GetEncoding("iso-8859-1")))
            {
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    if (s.IndexOf(nomeAntigo) > -1)
                    {
                        s = s.Replace(nomeAntigo, nomeNovo);
                    }
                    sb.AppendLine(s);
                }
                sr.Close();
            }
            StreamWriter sw = new StreamWriter(nomeArquivo, false, Encoding.GetEncoding("iso-8859-1"));
            sw.Write(sb);
            sw.Close();
        }
        
    }
}
