namespace PongGame
{
    public class Raquete
    {
        public int X { get; }
        public int Y { get; private set; }
        public int Largura { get; }
        public int Altura { get; }
        public int Velocidade { get; }

        // Retângulo ocupado pela raquete (usado no desenho e nas colisões)
        public Rectangle Limites => new Rectangle(X, Y, Largura, Altura);

        public Raquete(int x, int y, int largura, int altura, int velocidade)
        {
            X = x;
            Y = y;
            Largura = largura;
            Altura = altura;
            Velocidade = velocidade;
        }

        public void MoverParaCima()
        {
            // Não deixa passar da borda superior
            Y = Math.Max(0, Y - Velocidade);
        }

        public void MoverParaBaixo(int alturaArea)
        {
            // Não deixa passar da borda inferior
            Y = Math.Min(alturaArea - Altura, Y + Velocidade);
        }

        public void Centralizar(int alturaArea)
        {
            Y = (alturaArea - Altura) / 2;
        }
    }
}
