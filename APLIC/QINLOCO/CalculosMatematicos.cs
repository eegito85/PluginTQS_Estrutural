using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QINLOCO
{
    class CalculosMatematicos
    {
        //---->>> CALCULA O VOLUME EPS
        public double calculaVolumeEPS(double areaEstruturada, double alturaLajeNer, double volumeConcreto)
        {
            double volumeEps = 0;
            volumeEps = (areaEstruturada * (Math.Round(alturaLajeNer / 100, 2))) - volumeConcreto;
            volumeEps = Math.Round(volumeEps, 2);
            return volumeEps;
        }

        //---->>> CALCULA A RESISTÊNCIA DO CONCRETO (EM MPa)
        public double calculaResistencia(double fck, double s, double dias)
        {
            double resistencia = 0;
            double expoente = 0;
            expoente = Math.Round((s * (1 - Math.Sqrt(28 / dias))), 10);
            resistencia = fck * (Math.Exp(expoente));
            resistencia = Math.Round(resistencia, 2);
            return resistencia;
        }

        //---->>> CALCULA Eci
        public double calculaEci(double fck, double alfaE, double s, double dias)
        {
            double fator = 0;
            double expoente = 1 / 3;
            double expoente1 = 0;
            double resistencia = 0;
            double Eci = 0;

            expoente1 = s * (1 - Math.Sqrt(28 / dias));
            resistencia = fck * (Math.Pow(Math.E, expoente1));

            if (fck >= 20 && fck <= 50)
            {
                Eci = alfaE * 5.6 * (Math.Sqrt(resistencia));
            }
            else if (fck >= 55 && fck <= 90)
            {
                fator = (resistencia / 10) + 1.25;
                Eci = 21.5 * alfaE * (Math.Pow(fator, expoente));
            }

            Eci = Math.Round(Eci, 1);
            return Eci;
        }

        //---->>> CALCULA Ecs
        public double calculaEcs(double fck, double alfaE, double s, double dias)
        {
            double Eci = 0;
            double Ecs = 0;
            double fator = 0;

            Eci = calculaEci(fck, alfaE, s, dias);
            fator = 0.8 + ((0.2 * fck) / 80);
            if (fator < 1) { Ecs = fator * Eci; }
            else { Ecs = Eci; }

            Ecs = Math.Round(Ecs, 1);
            return Ecs;
        }

        public double calculaDEci(double fck, double alfaE)
        {
            double fator = 0;
            double expoente = 1 / 3;
            double Eci = 0;

            if (fck >= 20 && fck <= 50)
            {
                Eci = alfaE * 5.6 * (Math.Sqrt(fck));
            }
            else if (fck >= 55 && fck <= 90)
            {
                fator = (fck / 10) + 1.25;
                Eci = 21.5 * alfaE * (Math.Pow(fator, expoente));
            }

            Eci = Math.Round(Eci, 1);
            return Eci;
        }

        public double calculaDEcs(double fck, double alfaE)
        {
            double Eci = 0;
            double Ecs = 0;
            double fator = 0;

            Eci = calculaDEci(fck, alfaE);
            fator = 0.8 + ((0.2 * fck) / 80);
            if (fator < 1) { Ecs = fator * Eci; }
            else { Ecs = Eci; }

            Ecs = Math.Round(Ecs, 1);
            return Ecs;
        }

        //--->>> RETORNA O COBRIMENTO DA TABELA COM AS CORREÇÕES.
        public double retornaCobrimentoTotal(int indicadorControleQualidade, string tipoElemento, string cobrimentoAdotado, string classeAgressividade, double fck)
        {
            double c = 0;
            double cAdotado = Convert.ToDouble(cobrimentoAdotado);
            int indicadorClasse = 0;
            switch (classeAgressividade)
            {
                case "I-Fraca-Rural":
                    indicadorClasse = 1;
                    break;
                case "II-Moderada-Urbana":
                    indicadorClasse = 2;
                    break;
                case "III-Forte-Marinho":
                    indicadorClasse = 3;
                    break;
                case "IV-Muito forte-Industrial":
                    indicadorClasse = 4;
                    break;
            }

            if (cAdotado != 0) { c = Convert.ToDouble(cobrimentoAdotado); }
            else
            {
                c = retornaCobrimentoTabela(tipoElemento, indicadorClasse);
                // fator de correção quando se tem controle adequado de qualidade;
                if (indicadorControleQualidade == 1 && indicadorClasse != 4) { c = c - 0.5; }
                
                
            }
            
            return c;
        }

        //--->>> RETORNA O COBRIMENTO DA TABELA 7.2
        public double retornaCobrimentoTabela(string tipoElemento, int indicadorClasse)
        {
            double c = 0;

            if (tipoElemento == "BLOCOS/SAPATAS")
            {
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 4.5;
                            break;
                        case 2:
                            c = 4.5;
                            break;
                        case 3:
                            c = 4.5;
                            break;
                        case 4:
                            c = 5;
                            break;
                    }
            }
            else if (tipoElemento == "CORTINAS/MUROS")
            {
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 3;
                            break;
                        case 2:
                            c = 3;
                            break;
                        case 3:
                            c = 4;
                            break;
                        case 4:
                            c = 5;
                            break;
                    } 
            }
            else if (tipoElemento == "LAJE DA TAMPA")
            {
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 2;
                            break;
                        case 2:
                            c = 2.5;
                            break;
                        case 3:
                            c = 3.5;
                            break;
                        case 4:
                            c = 4.5;
                            break;
                    }
            }
            else if (tipoElemento == "PAREDES E LAJE DO FUNDO")
            {
                
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 2.5;
                            break;
                        case 2:
                            c = 3;
                            break;
                        case 3:
                            c = 4;
                            break;
                        case 4:
                            c = 5;
                            break;
                    }
            }
            else if (tipoElemento == "ARMADURA NEGATIVA")
            {
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 2;
                            break;
                        case 2:
                            c = 2.5;
                            break;
                        case 3:
                            c = 3.5;
                            break;
                        case 4:
                            c = 4.5;
                            break;
                    }
            }
            else if (tipoElemento == "ARMADURA POSITIVA")
            {
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 2;
                            break;
                        case 2:
                            c = 2.5;
                            break;
                        case 3:
                            c = 3.5;
                            break;
                        case 4:
                            c = 4.5;
                            break;
                    }
            }
            else if (tipoElemento == "ESCADAS E RAMPAS")
            {
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 2;
                            break;
                        case 2:
                            c = 2.5;
                            break;
                        case 3:
                            c = 3.5;
                            break;
                        case 4:
                            c = 4.5;
                            break;
                    }
            }
            else if (tipoElemento == "VIGAS DE BALDRAME")
            {
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 3;
                            break;
                        case 2:
                            c = 3;
                            break;
                        case 3:
                            c = 4;
                            break;
                        case 4:
                            c = 5;
                            break;
                    }
            }
            else if (tipoElemento == "DEMAIS VIGAS")
            {              
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 2.5;
                            break;
                        case 2:
                            c = 3;
                            break;
                        case 3:
                            c = 4;
                            break;
                        case 4:
                            c = 5;
                            break;
                    }
            }
            else if (tipoElemento == "PILARES EM CONTATO COM O SOLO")
            {
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 4.5;
                            break;
                        case 2:
                            c = 4.5;
                            break;
                        case 3:
                            c = 4.5;
                            break;
                        case 4:
                            c = 5;
                            break;
                    }
            }
            else if (tipoElemento == "DEMAIS PILARES")
            {
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 2.5;
                            break;
                        case 2:
                            c = 3;
                            break;
                        case 3:
                            c = 4;
                            break;
                        case 4:
                            c = 5;
                            break;
                    }  
            }
            else if (tipoElemento == "VIGAS")
            {
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 3;
                            break;
                        case 2:
                            c = 3.5;
                            break;
                        case 3:
                            c = 4.5;
                            break;
                        case 4:
                            c = 5.5;
                            break;
                    }
            }
            else if (tipoElemento == "LAJES")
            {
                    switch (indicadorClasse)
                    {
                        case 1:
                            c = 2.5;
                            break;
                        case 2:
                            c = 3;
                            break;
                        case 3:
                            c = 4;
                            break;
                        case 4:
                            c = 5;
                            break;
                    } 
            }

            return c;
        }

        //--->>> RETORNA A CLASSE DO CONCRETO SEGUNDO A TABELA 7.1
        //public double retornaClasseConcreto(int indicadorClasse)
        //{
        //    double classe = 0;
        //
        //    switch (indicadorClasse)
        //    {
        //        case 1:
        //            classe = 25;
        //            break;
        //        case 2:
        //            classe = 30;
        //            break;
        //        case 3:
        //            classe = 35;
        //            break;
        //        case 4:
        //            classe = 40;
        //            break;
        //    }
        //    return classe;
        //}

        public double fatorCorrecaoCobrimentoCA(double fck, string classeAgressividade)
        {
            double fator = 0;
            if (classeAgressividade == "I-Fraca-Rural" && fck > 20)
            {
                fator = 0.5;
            }
            if (classeAgressividade == "II-Moderada-Urbana" && fck > 25)
            {
                fator = 0.5;
            }
            if (classeAgressividade == "III-Forte-Marinho" && fck > 30)
            {
                fator = 0.5;
            }
            if (classeAgressividade == "IV-Muito forte-Industrial" && fck > 40)
            {
                fator = 0.5;
            }
            return fator;

        }

        public double fatorCorrecaoCobrimentoCP(double fck, string classeAgressividade)
        {
            double fator = 0;
            if (classeAgressividade == "I-Fraca-Rural" && fck > 25)
            {
                fator = 0.5;
            }
            if (classeAgressividade == "II-Moderada-Urbana" && fck > 30)
            {
                fator = 0.5;
            }
            if (classeAgressividade == "III-Forte-Marinho" && fck > 35)
            {
                fator = 0.5;
            }
            if (classeAgressividade == "IV-Muito forte-Industrial" && fck > 40)
            {
                fator = 0.5;
            }
            return fator;

        }

        public string mensagemAlertaFck(double fck, string classeAgressividade)
        {
            string msg = "";

            if (classeAgressividade == "I-Fraca-Rural" && fck < 20)
            {
                msg = "fck de projeto menor que o mínimo (20 MPa)";
            }
            if (classeAgressividade == "I-Fraca-Rural" && fck >= 20 && fck < 25)
            {
                msg = "fck de projeto menor que o mínimo para concreto protendido (25 MPa)";
            }
            if (classeAgressividade == "II-Moderada-Urbana" && fck < 25)
            {
                msg = "fck de projeto menor que o mínimo (25 MPa)";
            }
            if (classeAgressividade == "II-Moderada-Urbana" && fck >= 25 && fck < 30)
            {
                msg = "fck de projeto menor que o mínimo para concreto protendido (30 MPa)";
            }

            if (classeAgressividade == "III-Forte-Marinho" && fck < 30)
            {
                msg = "fck de projeto menor que o mínimo (30 MPa)";
            }
            if (classeAgressividade == "III-Forte-Marinho" && fck >= 30 && fck < 35)
            {
                msg = "fck de projeto menor que o mínimo para concreto protendido (35 MPa)";
            }

            if (classeAgressividade == "IV-Muito forte-Industrial" && fck < 40)
            {
                msg = "fck de projeto menor que o mínimo (40 MPa)";
            }

            return msg;
        }

    }
}
