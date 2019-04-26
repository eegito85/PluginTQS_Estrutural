using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace QINLOCO
{
    class ManipulacaoArquivos
    {
        //--->>>> FUNÇÃO QUE PEGA OS NOMES DE TODOS OS PAVIMENTOS
        public List<string> lerPavimentos(string caminhoPasta)
        {
            //string arquivo = @"C:\TQS\002_RESID\ESPACIAL\CARDWG.LST";
            string arquivo = caminhoPasta + "\\" + "ESPACIAL" + "\\" + "CARDWG.LST";
            List<string> listaPavimentos = new List<string>();
            if (File.Exists(arquivo))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
                    {
                        String linha;
                        string nomePavimento = "";

                        // Lê linha por linha até o final do arquivo
                        while ((linha = sr.ReadLine()) != null)
                        {
                            //MessageBox.Show(linha, "Teste",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            if (linha != "")
                            {
                                string primeiraPalavra = linha.Substring(0, 12);
                                primeiraPalavra = primeiraPalavra.TrimStart();
                                primeiraPalavra = primeiraPalavra.TrimEnd();

                                if (primeiraPalavra == "Planta")
                                {
                                    int comprimentoLinha = linha.Length;
                                    int posicao = linha.IndexOf("Planta");
                                    nomePavimento = linha.Substring(posicao + 7, (comprimentoLinha - posicao - 7));
                                    //MessageBox.Show(nomePavimento, "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    listaPavimentos.Add(nomePavimento);
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ler Pavimentos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(" O arquivo " + arquivo + "não foi localizado !", "QINLOCO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return listaPavimentos;
        }

        //--->>>> FUNÇÃO QUE LÊ OS DADOS PARA UM DETERMINADO PAVIMENTO
        public string[] lerDadosPavimento(string nomePavimento, string caminhoPasta)
        {
            //string arquivo = @"C:\TQS\002_RESID\" + nomePavimento + "\\" + nomePavimento + ".LST";
            string arquivo = caminhoPasta + nomePavimento + "\\" + nomePavimento + ".LST";
            string linhaSelecionada = "";
            List<string> listaDadosPavimento = new List<string>();
            string[] dados = new string[6];
            string[] info = new string[7];
            if (File.Exists(arquivo))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
                    {
                        String linha;
                        int controlador = 0;
                        // Lê linha por linha até o final do arquivo
                        while ((linha = sr.ReadLine()) != null)
                        {
                            if (linha != "" && controlador == 1)
                            {
                                linhaSelecionada = linha;
                                break;
                            }

                            if (linha != "" && controlador == 0)
                            {
                                string primeiraPalavra = linha.TrimStart();
                                primeiraPalavra = primeiraPalavra.Substring(0, 4);

                                if (primeiraPalavra == "Piso")
                                {
                                    controlador = 1;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ler Dados Pavimentos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(" O arquivo " + arquivo + "não foi localizado !", "QINLOCO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (linhaSelecionada != "")
            {
                linhaSelecionada.TrimStart();
                linhaSelecionada.TrimEnd();
                string[] stringSeparadores = new string[] { " " };
                char[] charSeparadores = new char[] { ' ' };
                dados = linhaSelecionada.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);
                //for (int i = 0; i < dados.Length; i++)
                //{
                //    MessageBox.Show(dados[i], "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //}
                //MessageBox.Show(linhaSelecionada, "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            string[] dadosGrefor = lerGrefor(nomePavimento, caminhoPasta);
            dados[4] = dadosGrefor[0];
            dados[5] = dadosGrefor[1];
            string espessura = lerEspessuraMedia(nomePavimento, caminhoPasta);
            info[0] = dados[1];
            info[1] = dados[4];
            info[2] = dados[0];
            info[3] = dados[2];
            info[4] = dados[3];
            info[5] = espessura;
            info[6] = dados[5];
            return info;
        }

        public string[] lerDadosProjeto(string caminhoPasta)
        {
            //string arquivo = @"C:\TQS\002_RESID\ESPACIAL\CARDWG.LST";
            string arquivo = caminhoPasta + "\\" + "ESPACIAL" + "\\" + "PORALF.LST";
            string[] dadosProjeto = new string[3] { "", "", "" };
            if (File.Exists(arquivo))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
                    {
                        String linha;
                        string numeroProjeto = "";
                        string tituloProjeto = "";
                        string nomeCliente = "";

                        // Lê linha por linha até o final do arquivo
                        while ((linha = sr.ReadLine()) != null)
                        {
                            //MessageBox.Show(linha, "Teste",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            if (linha != "")
                            {
                                
                                linha = linha.TrimStart();
                                linha = linha.TrimEnd();
                                string primeiraPalavra = linha.Substring(0, 7);
                                string primeiraLetra = linha.Substring(0, 1);

                                if (primeiraPalavra == "Projeto")
                                {
                                    char[] charSeparadores = new char[] { ' ' };
                                    string[] dados = linha.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);
                                    numeroProjeto = dados[1];
                                    dadosProjeto[0] = numeroProjeto;
                                    //MessageBox.Show(dados[1], "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                if (primeiraPalavra == "Pórtico")
                                {
                                    char[] charSeparadores = new char[] { ' ' };
                                    string[] dados1 = linha.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);
                                    int n = dados1.Length;
                                    for (int i = 1; i < n; i++)
                                    {
                                        if(i==1) { tituloProjeto = dados1[i]; }
                                        else { tituloProjeto = tituloProjeto + " " + dados1[i]; }
                                    }
                                    //tituloProjeto = dados1[1]+dados1[2];
                                    tituloProjeto = tiraAspas(tituloProjeto);
                                    dadosProjeto[1] = tituloProjeto;
                                    //MessageBox.Show(dados1[1], "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                if (primeiraLetra == "'")
                                {
                                    nomeCliente = linha;
                                    nomeCliente = tiraAspas(nomeCliente);
                                    dadosProjeto[2] = nomeCliente;
                                    //MessageBox.Show(linha, "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    break;
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ler Dados projeto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(" O arquivo " + arquivo + "não foi localizado !", "QINLOCO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dadosProjeto;
        }

        //--->>>> FUNÇÃO QUE LÊ O ARQUIVO GREFOR PARA UM DETERMINADO PAVIMENTO
        public string[] lerGrefor(string nomePavimento, string caminhoPasta)
        {
            //string arquivo = @"C:\TQS\002_RESID\" + nomePavimento + "\\GREFOR.LST";
            string arquivo = caminhoPasta + nomePavimento + "\\GREFOR.LST";
            List<string> listaDadosPavimento = new List<string>();
            string fck = "0";
            string E = "0";
            string p1 = "";
            string p2 = "";
            string p3 = "";
            string[] dados = new string[2];
            if (File.Exists(arquivo))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
                    {
                        String linha;
                        // Lê linha por linha até o final do arquivo
                        while ((linha = sr.ReadLine()) != null)
                        {
                            if (linha != "")
                            {
                                linha.TrimStart();
                                linha.TrimEnd();
                                p1 = linha.Substring(0, 5);
                                p2 = linha.Substring(0, 6);
                                if(p1=="Valor")
                                {
                                    char[] charSeparadores = new char[] { ' ' };
                                    string[] elems = linha.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);
                                    fck = elems[2];
                                    fck = ajustarFck(fck);
                                    //MessageBox.Show(fck, "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                //MessageBox.Show(linha, "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                //p1 = linha.Substring(0, 3);
                                //p2 = linha.Substring(0, 6);
                                //if (p1 == "FCK" && linha != "FCK definido no edifício")
                                //{
                                //    fck = linha.Substring(54, 6);
                                //    fck = fck.TrimStart();
                                //    fck = fck.TrimEnd();
                                //    fck = ajustarFck(fck);
                                //}
                                if(p2=="Módulo")
                                {
                                    char[] cS = new char[] { ' ' };
                                    string[] elms = linha.Split(cS, StringSplitOptions.RemoveEmptyEntries);
                                    int qtdE = elms.Length;
                                    p3 = elms[3];
                                    if (p3 == "longitudinal")
                                    {
                                        E = elms[qtdE - 3];
                                        //E = linha.Substring(51, 9);
                                        //E = E.TrimStart();
                                        //E = E.TrimEnd();
                                        //MessageBox.Show(E, "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "fck e módulo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(" O arquivo " + arquivo + "não foi localizado !", "QINLOCO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dados[0] = fck;
            dados[1] = E;
            return dados;
        }

        //--->>>> FUNÇÃO QUE LÊ A ESPESSURA MÉDIA PARA UM DETERMINADO PAVIMENTO
        public string lerEspessuraMedia(string nomePavimento, string caminhoPasta)
        {
            //string arquivo = @"C:\TQS\002_RESID\" + nomePavimento + "\\" + nomePavimento + ".LST";
            string arquivo = caminhoPasta + nomePavimento + "\\" + nomePavimento + ".LST";
            string p = "";
            string espessura = "0";
            if (File.Exists(arquivo))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
                    {
                        String linha;
                        
                        // Lê linha por linha até o final do arquivo
                        while ((linha = sr.ReadLine()) != null)
                        {
                            if (linha != "")
                            {
                                linha = linha.TrimStart();
                                char[] charSeparadores = new char[] { ' ' };
                                string[] elems = linha.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);
                                p = elems[0];
                                if (p == "Espessura")
                                {
                                    espessura = elems[5];
                                    //espessura = linha.Substring(30, 6);
                                    espessura.TrimStart();
                                    espessura.TrimEnd();
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ler Espessura Média", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(" O arquivo " + arquivo + "não foi localizado !", "QINLOCO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return espessura;
        }

        //--->>>> FUNÇÃO QUE LÊ O QUANTITATIVO DE VIGAS PARA UM DETERMINADO PAVIMENTO
        public List<string> lerQuantitativoViga(string nomePavimento, string caminhoPasta)
        {
            //string arquivo = @"C:\TQS\002_RESID\" + nomePavimento + "\\" + nomePavimento + ".LST";
            string arquivo = caminhoPasta + nomePavimento + "\\" + nomePavimento + ".LST";
            List<string> listaDadosViga = new List<string>();
            int qtd = 0;
            string primeiraP = "";
            string primeiraLetra = "";
            
            if (File.Exists(arquivo))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
                    {
                        String linha;
                        int controlador = 0;
                        // Lê linha por linha até o final do arquivo
                        while ((linha = sr.ReadLine()) != null)
                        {
                            linha = linha.TrimStart();
                            linha = linha.TrimEnd();
                            //MessageBox.Show(linha, "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            if (controlador == 1 && linha != "")
                            {
                                primeiraLetra = linha.Substring(0, 1);
                                char[] charSeparadores = new char[] { ' ' };
                                string[] elems = linha.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);
                                string primeiraPalavra = elems[0];
                                if (primeiraLetra=="V")
                                {
                                    if (primeiraPalavra != "Volume")
                                    {
                                        listaDadosViga.Add(linha);
                                    }
                                }
                                if (primeiraLetra == "-")
                                {
                                    controlador = 2;
                                    break;
                                }
                            }

                            if (controlador == 0)
                            {
                                if (linha != "")
                                {
                                    qtd = linha.Length;
                                    if (qtd > 8)
                                    {
                                        primeiraP = linha.Substring(0, 8);
                                        if (primeiraP == "Elemento")
                                        {
                                            controlador = 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Qtd Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(" O arquivo " + arquivo + "não foi localizado !", "QINLOCO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return listaDadosViga;
        }

        //--->>>> FUNÇÃO QUE LÊ O QUANTITATIVO DE PILARES PARA UM DETERMINADO PAVIMENTO
        public List<string> lerQuantitativoPilares(string nomePavimento, string caminhoPasta)
        {
            //string arquivo = @"C:\TQS\002_RESID\" + nomePavimento + "\\" + nomePavimento + ".LST";
            string arquivo = caminhoPasta + nomePavimento + "\\" + nomePavimento + ".LST";
            List<string> listaDadosPilares = new List<string>();
            int qtd = 0;
            string primeiraP = "";
            string primeiraLetra = "";

            if (File.Exists(arquivo))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
                    {
                        String linha;
                        int controlador = 0;
                        // Lê linha por linha até o final do arquivo
                        while ((linha = sr.ReadLine()) != null)
                        {
                            linha = linha.TrimStart();
                            linha = linha.TrimEnd();
                            //MessageBox.Show(linha, "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            if (controlador == 1 && linha != "")
                            {
                                primeiraLetra = linha.Substring(0, 1);
                                if (primeiraLetra == "P")
                                {
                                    listaDadosPilares.Add(linha);
                                }
                                if (primeiraLetra == "-")
                                {
                                    controlador = 2;
                                    break;
                                }
                            }

                            if (controlador == 0)
                            {
                                if (linha != "")
                                {
                                    qtd = linha.Length;
                                    if (qtd > 6)
                                    {
                                        primeiraP = linha.Substring(0, 4);
                                        if (primeiraP == "topo")
                                        {
                                            controlador = 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Qtd Pilares", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(" O arquivo " + arquivo + "não foi localizado !", "QINLOCO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return listaDadosPilares;
        }

        //--->>>> FUNÇÃO PARA TRANSFORMAR FCK PARA MPa
        private string ajustarFck(string fck)
        {
            fck = fck.TrimStart();
            fck = fck.TrimEnd();
            string[] numeros = fck.Split('.');
            string p1 = numeros[0];
            string p2 = numeros[1];
            if (numeros[0] == "") { p1 = "0"; }
            string numero = p1 + "," + p2;
            double nfck = Convert.ToDouble(numero);
            nfck = Math.Round((nfck / 10), 2);
            numero = Convert.ToString(nfck);
            return numero;
        }

        //--->>>> FUNÇÃO QUE LÊ O QUANTITATIVO DE LAJES PARA UM DETERMINADO PAVIMENTO
        public List<string> lerQuantitativoLajes(string nomePavimento, string caminhoPasta)
        {
            //string arquivo = @"C:\TQS\002_RESID\" + nomePavimento + "\\" + nomePavimento + ".LST";
            string arquivo = caminhoPasta + nomePavimento + "\\" + nomePavimento + ".LST";
            List<string> listaDadosLaje = new List<string>();
            int qtd = 0;
            string primeiraP = "";
            string primeiraLetra = "";
            //string f = "";

            if (File.Exists(arquivo))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
                    {
                        String linha;
                        int controlador = 0;
                        // Lê linha por linha até o final do arquivo
                        while ((linha = sr.ReadLine()) != null)
                        {
                            linha = linha.TrimStart();
                            linha = linha.TrimEnd();
                            //MessageBox.Show(linha, "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            if (controlador == 1 && linha != "")
                            {
                                primeiraLetra = linha.Substring(0, 1);
                                if (primeiraLetra == "L")
                                {
                                    listaDadosLaje.Add(linha);
                                }
                                if (primeiraLetra == "T")
                                {
                                    controlador = 2;
                                    break;
                                }
                            }

                            if (controlador == 0)
                            {
                                if (linha != "")
                                {
                                    qtd = linha.Length;
                                    if (qtd > 6)
                                    {
                                        primeiraP = linha.Substring(0, 4);
                                        if (primeiraP == "topo")
                                        {
                                            controlador = 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Qtd Lajes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(" O arquivo " + arquivo + "não foi localizado !", "QINLOCO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return listaDadosLaje;
        }

        //--->>>> FUNÇÃO QUE LÊ O QUANTITATIVO DE LAJES NERVURADAS PARA UM DETERMINADO PAVIMENTO
        public string retornaAlturaLajesNervuradas(string nomePavimento, string nomeLaje, string caminhoPasta)
        {
            string numeroLaje = nomeLaje.Substring(1);
            string alturaLaje = "";
            string alturaCapa = "";
            double alturaTotal = 0;
            string altura = "";
            //string arquivo = @"C:\TQS\002_RESID\" + nomePavimento + "\\" + nomePavimento + ".LST";
            string arquivo = caminhoPasta + nomePavimento + "\\" + nomePavimento + ".LST";
            int qtd = 0;
            string primeiraP = "";
            string primeiraLetra = "";
            int indicadorLajeNervurada = 0; //0 se for laje maçica 1 se for laje nervurada

            if (File.Exists(arquivo))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
                    {
                        String linha;
                        int controlador = 0;
                        // Lê linha por linha até o final do arquivo
                        while ((linha = sr.ReadLine()) != null)
                        {
                            linha = linha.TrimStart();
                            linha = linha.TrimEnd();
                            //MessageBox.Show(linha, "Teste", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            if (controlador == 1 && linha != "")
                            {
                                primeiraLetra = linha.Substring(0, 1);
                                if (primeiraLetra != "L" && primeiraLetra != "-" && primeiraLetra != "T")
                                {
                                    char[] charSeparadores = new char[] { ' ' };
                                    string[] dados = linha.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);
                                    if (dados[0] == numeroLaje)
                                    {
                                        alturaLaje = dados[1];
                                        alturaCapa = dados[2];
                                        alturaTotal = converteEmNumero(alturaLaje) + converteEmNumero(alturaCapa);
                                        altura = Convert.ToString(alturaTotal);
                                        indicadorLajeNervurada = 1;
                                        break;
                                    }
                                }
                                if (primeiraLetra == "T")
                                {
                                    controlador = 2;
                                    break;
                                }
                            }

                            if (controlador == 0)
                            {
                                if (linha != "")
                                {
                                    qtd = linha.Length;
                                    if (qtd > 15)
                                    {
                                        primeiraP = linha.Substring(0, 16);
                                        if (primeiraP == "Lajes Nervuradas")
                                        {
                                            controlador = 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Altura Laje nervurada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(" O arquivo " + arquivo + "não foi localizado !", "QINLOCO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if(indicadorLajeNervurada==0) { alturaLaje = "0.00"; }
            return altura;
        }

        //---->>> CALCULA O VOLUME EPS
        public double calculaVolumeEPS(double areaEstruturada, double alturaLajeNer, double volumeConcreto)
        {
            double volumeEps = 0;
            volumeEps = (areaEstruturada * (Math.Round(alturaLajeNer / 100, 2))) - volumeConcreto;
            volumeEps = Math.Round(volumeEps, 2);
            return volumeEps;
        }

        //--->>> PEGA A PASTA ONDE ESTÃO OS ARQUIVOS DO PROJETO
        public string retornaCaminhoPasta(string caminhoArquivo)
        {
            string caminhoPasta = "";
            string[] pastas = caminhoArquivo.Split('\\');
            caminhoPasta = pastas[0] + "\\" + pastas[1] + "\\" + pastas[2] + "\\";
            return caminhoPasta;
        }

        public List<string> retornaListaSomatorios(string nomePavimento, string caminhoPasta)
        {
            List<string> lista = new List<string>();
            List<string> listaVigas = lerQuantitativoViga(nomePavimento, caminhoPasta);
            List<string> listaPilares = lerQuantitativoPilares(nomePavimento, caminhoPasta);
            List<string> listaLajes = lerQuantitativoLajes(nomePavimento, caminhoPasta);

            int qtdListaVigas = listaVigas.Count();
            int qtdListaPilares = listaPilares.Count();
            int qtdListaLajes = listaLajes.Count();

            double somaAeV = 0;
            double somaAeP = 0;
            double somaAeL = 0;

            double somaAfV = 0;
            double somaAfP = 0;
            double somaAfL = 0;

            double somaVcV = 0;
            double somaVcP = 0;
            double somaVcL = 0;

            //double somaEPS = 0;

            double somaAe = 0;
            double somaAf = 0;
            double somaVc = 0;

            string[] dados = new string[6];

            if (qtdListaVigas > 0)
            {
                foreach (string dado in listaVigas)
                {
                    dado.TrimStart();
                    dado.TrimEnd();
                    char[] charSeparadores = new char[] { ' ' };
                    dados = dado.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);

                    somaAeV = somaAeV + converteEmNumero(dados[1]);
                    somaAfV = somaAfV + converteEmNumero(dados[2]);
                    somaVcV = somaVcV + converteEmNumero(dados[3]);
                }
            }

            if (qtdListaPilares > 0)
            {
                foreach (string dado in listaPilares)
                {
                    dado.TrimStart();
                    dado.TrimEnd();
                    char[] charSeparadores = new char[] { ' ' };
                    dados = dado.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);

                    somaAeP = somaAeP + converteEmNumero(dados[1]);
                    somaAfP = somaAfP + converteEmNumero(dados[2]);
                    somaVcP = somaVcP + converteEmNumero(dados[3]);
                }
            }

            if (qtdListaLajes > 0)
            {
                foreach (string dado in listaLajes)
                {
                    dado.TrimStart();
                    dado.TrimEnd();
                    char[] charSeparadores = new char[] { ' ' };
                    dados = dado.Split(charSeparadores, StringSplitOptions.RemoveEmptyEntries);

                    somaAeL = somaAeL + converteEmNumero(dados[1]);
                    somaAfL = somaAfL + converteEmNumero(dados[2]);
                    somaVcL = somaVcL + converteEmNumero(dados[3]);
                }
            }
            somaAe = somaAeV + somaAeP + somaAeL;
            somaAf = somaAfV + somaAfP + somaAfL;
            somaVc = somaVcV + somaVcP + somaVcL;

            lista.Add(Convert.ToString(somaAe));
            lista.Add(Convert.ToString(somaAf));
            lista.Add(Convert.ToString(somaVc));

            lista.Add(Convert.ToString(somaAfV));
            lista.Add(Convert.ToString(somaAfP));
            lista.Add(Convert.ToString(somaAfL));

            lista.Add(Convert.ToString(somaVcV));
            lista.Add(Convert.ToString(somaVcP));
            lista.Add(Convert.ToString(somaVcL));

            return lista;

        }

        public double converteEmNumero(string num)
        {
            num = num.TrimStart();
            num = num.TrimEnd();
            string[] numeros = new string[2];
            string p1 = "";
            string p2 = "";
            string numero = "";
            string ponto = ".";
            string virgula = ",";
            bool vPonto = num.Contains(ponto);
            bool vVirgula = num.Contains(virgula);
            //caso o número decimal tenha ponto
            if (vPonto == true && vVirgula == false)
            {
                numeros = num.Split('.');
                p1 = numeros[0];
                p2 = numeros[1];
                if (numeros[0] == "") { p1 = "0"; }
                numero = p1 + "," + p2;
            }
            else if (vPonto == false && vVirgula == true)
            {
                numero = num;
            }
            else if (vPonto == false && vVirgula == false)
            {
                numero = num + ",00";
            }
            else
            {
                MessageBox.Show("O número digitado está no formato incorreto", "Número Inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numero = "0,00";
            }

            double f = Convert.ToDouble(numero);
            //string temp = num.Replace('.', ',');
            //double f = Convert.ToDouble(temp);
            //double f = Double.Parse(num);
            return f;
        }

        public string tiraAspas(string nome)
        {
            try
            {
                int tamanho = nome.Length;
                nome = nome.Substring(0, tamanho - 1);
                //Elimina o primeiro caracter da string
                nome = nome.Substring(1);
            }
            catch { }
            return nome;
        }
    }
}

