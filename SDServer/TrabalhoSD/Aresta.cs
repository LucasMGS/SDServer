using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrabalhoSD
{
    public class Aresta
    {
        public int Partida { get; set; }
        public int Destino { get; set; }
        public int Custo { get; set; }

        public Aresta(int Partida, int Destino, int Custo)
        {
            this.Partida = Partida;
            this.Destino = Destino;
            this.Custo = Custo;
        }

        public IEnumerable<int> ObterPesosDeTodosVertices(List<Aresta> Grafo)
        {
            for (int i = 0; i < Grafo.Count; i++)
            {
                yield return Grafo[i].Custo;
            }
        }
    }
}
