namespace PongGame
{
    public class Bola
    {
        private readonly Random _aleatorio = new Random();
        private readonly double _velocidadeInicial;
        private readonly double _velocidadeMaxima;

        // double evita perda de precisão no movimento
        public double X { get; private set; }
        public double Y { get; private set; }
        public double XAnterior { get; private set; }
        public double YAnterior { get; private set; }
        public double VelocidadeX { get; private set; }
        public double VelocidadeY { get; private set; }
        public int Tamanho { get; }

        public Rectangle Limites =>
            new Rectangle((int)Math.Round(X), (int)Math.Round(Y), Tamanho, Tamanho);

        public Bola(int tamanho, double velocidadeInicial, double velocidadeMaxima)
        {
            Tamanho = tamanho;
            _velocidadeInicial = velocidadeInicial;
            _velocidadeMaxima = velocidadeMaxima;
        }

        // Coloca a bola no centro da área e sorteia uma nova direção
        public void Centralizar(int larguraArea, int alturaArea)
        {
            X = (larguraArea - Tamanho) / 2.0;
            Y = (alturaArea - Tamanho) / 2.0;
            XAnterior = X;
            YAnterior = Y;

            // Ângulo entre 20 e 45 graus: evita trajetórias quase retas ou verticais
            double angulo = _aleatorio.Next(20, 46) * Math.PI / 180;
            int sentidoX = _aleatorio.Next(2) == 0 ? -1 : 1;
            int sentidoY = _aleatorio.Next(2) == 0 ? -1 : 1;

            VelocidadeX = Math.Cos(angulo) * _velocidadeInicial * sentidoX;
            VelocidadeY = Math.Sin(angulo) * _velocidadeInicial * sentidoY;
        }

        public void Mover()
        {
            XAnterior = X;
            YAnterior = Y;
            X += VelocidadeX;
            Y += VelocidadeY;
        }

        // Colisão com raquete: inverte a direção horizontal e acelera um pouco
        public void RebaterHorizontal(double novoX)
        {
            X = novoX;

            double velocidade = Math.Sqrt(VelocidadeX * VelocidadeX + VelocidadeY * VelocidadeY);
            double fator = Math.Min(1.05, _velocidadeMaxima / velocidade);

            VelocidadeX = -VelocidadeX * fator;
            VelocidadeY *= fator;
        }

        // Colisão com as bordas: inverte a direção vertical
        public void RebaterVertical(double novoY)
        {
            Y = novoY;
            VelocidadeY = -VelocidadeY;
        }
    }
}
