using System;
using System.Collections.Generic;
using System.Text;

namespace TrabalhoSD
{
    public class Grafo
    {
        public int QntdVertices;
        public int QntdArestas;
        public Aresta[] Aresta;

        public Grafo(int qntdVertices,int qntdArestas)
        {
            QntdVertices = qntdVertices;
            QntdArestas = qntdArestas;
            Aresta = new Aresta[qntdArestas];
        }

        private static int[,] criarMatrizAdj(int tamanho)
        {
            int[,] matrizAdj = new int[tamanho, tamanho];
            var random = new Random();

            for (var i = 0; i < matrizAdj.GetLongLength(0); i++)
            {
                for (var j = 0; j < matrizAdj.GetLongLength(1); j++)
                {
                    if (i == j)
                    {
                        matrizAdj[i, j] = 0;
                    }
                    else
                    {
                        int randomNumber = random.Next(2);
                        matrizAdj[i, j] = randomNumber;
                    }
                }
            }
            return matrizAdj;
        }

        public static Grafo Preencher(int qntdVertices, int qntdArestas)
        {
            var grafo = new Grafo(qntdVertices, qntdArestas);
            for (int i = 0; i < qntdVertices; i++)
            {
                grafo.Aresta[i].Partida = i;

            }
            return grafo;
            
        }
    }
}
