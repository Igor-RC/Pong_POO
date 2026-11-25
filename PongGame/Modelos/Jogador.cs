namespace PongGame
{
    public class Jogador
    {
        public string Nome { get; }
        public int Pontuacao { get; private set; }
        public Raquete Raquete { get; }

        public Jogador(string nome, Raquete raquete)
        {
            Nome = nome;
            Raquete = raquete;
            Pontuacao = 0;
        }

        public void MarcarPonto()
        {
            Pontuacao++;
        }

        public void ZerarPontuacao()
        {
            Pontuacao = 0;
        }
    }
}
