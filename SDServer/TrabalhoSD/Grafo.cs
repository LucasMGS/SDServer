using System;
using System.Collections.Generic;
using System.Text;

namespace TrabalhoSD
{
    public class Grafo
    {
        public int QntdVertices { get; set; }
        public int QntdArestas { get; set; }
        private int[,] MatrizAdjacencia { get; set; }
        public static List<Aresta> Arestas = new List<Aresta>();

        public Grafo(int qntdVertices)
        {
            QntdVertices = qntdVertices;
            MatrizAdjacencia = CriarMatrizAdj(qntdVertices);
            Arestas = CriarGrafo(MatrizAdjacencia);
            QntdArestas = Arestas.Count;
        }

        private int[,] CriarMatrizAdj(int tamanho)
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

        private List<Aresta> CriarGrafo(int[,] matrizAdj)
        {
            var grafo = new List<Aresta>();
            var random = new Random();
            var tam = matrizAdj.GetLongLength(0);
            for (int i = 0; i < tam; i++)
            {
                for (int j = i + 1; j < tam; j++) // percorre o triangulo superior da matriz de adjacencia
                {
                    if (matrizAdj[i, j] == 1)
                    {
                        var rangeCusto = Int32.Parse(tam.ToString());
                        var custo = random.Next(rangeCusto);
                        if (matrizAdj[i, j] == 1)
                        {
                            grafo.Add(new Aresta(i, j, custo));
                            grafo.Add(new Aresta(j, i, custo));
                        }
                    }
                }
            }
            return grafo;
        }

        public void Mostrar()
        {
            foreach (var aresta in Arestas)
            {
                Console.WriteLine("Vértice {0} se conecta ao vértice {1} com custo {2}.", aresta.Partida, aresta.Destino, aresta.Custo);
            }
        }
    }   
}
