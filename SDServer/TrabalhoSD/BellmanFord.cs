using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TrabalhoSD;

namespace BellmanFordAlgorithm
{
    class BellmanFord
    {
      
        public static string Buscar(Grafo grafo, int noPartida, int noDestino)
        {
            int qntdVertices = grafo.QntdVertices;
            int qntdArestas = grafo.QntdArestas;
            int[] distancia = new int[qntdVertices];
            bool contemCicloNegativo = false;

            for (int i = 0; i < qntdVertices; i++)
                distancia[i] = int.MaxValue;

            distancia[noPartida] = 0;

            for (int i = 1; i <= qntdVertices - 1; ++i)
            {
                for (int j = 0; j < qntdArestas; ++j)
                {                   
                    var arestasFiltradas = Grafo.Arestas.Where(x => x.Partida == j).ToList();
                    foreach (var aresta in arestasFiltradas)
                    {
                        int u = aresta.Partida;
                        int v = aresta.Destino; 
                        int peso = aresta.Custo; 

                        if (distancia[u] != int.MaxValue && distancia[u] + peso < distancia[v])
                            distancia[v] = distancia[u] + peso;
                    }
                }
            }

            for (int i = 0; i < qntdArestas; ++i)
            {
                var arestasFiltradas = Grafo.Arestas.Where(x => x.Partida == i).ToList();
                foreach (var aresta in arestasFiltradas)
                {
                    int u = aresta.Partida;
                    int v = aresta.Destino;
                    int peso = aresta.Custo;

                    if (distancia[u] != int.MaxValue && distancia[u] + peso < distancia[v])
                    {
                        contemCicloNegativo = true;
                    }
                }

                if (contemCicloNegativo)
                {
                    return "Este grafo possui contém ciclo negativo!";
                }
            }

                return MenorCaminhoEspecifico(distancia, qntdVertices,noPartida,noDestino);
                //MenorCaminho(distancia, qntdVertices, noPartida);
        }

        private static string MenorCaminhoEspecifico(int[] distancias,int qntdVertices,int noPartida,int noDestino)
        {

            for (int vertice = 0; vertice < qntdVertices; vertice++)
            {
                if (vertice == noDestino)
                {
                    if (distancias[vertice] != int.MaxValue)
                    {
                        return string.Format("A menor distância do vértice {0} para o vértice {1} é {2}",noPartida, noDestino, distancias[vertice]);
                    }
                    else
                    {
                       return string.Format("Não há uma conexão do vértice {0} com o vértice {1}", noPartida, noDestino);
                    }
                }
                else
                {
                    return string.Format("Vértice {0} não encontrado", noPartida);
                }
            }
            return "Busca Finalizada!";
        }
        private static void MenorCaminho(int[] distancia, int qntdVertices, int noPartida)
        {
            Console.WriteLine("Vertice   Distancia do vertice {0}",noPartida);

            for (int i = 0; i < qntdVertices; ++i)
            {
                Console.WriteLine("{0}\t \t{1}", i, distancia[i]);
            }
        }
    }
}