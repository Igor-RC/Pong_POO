using System.Drawing.Drawing2D;

namespace PongGame
{
    // Área de jogo: apenas desenha o estado recebido
    public class PainelJogo : Panel
    {
        private EstadoJogo? _estado;

        public PainelJogo()
        {
            DoubleBuffered = true; // evita que a imagem pisque
            BackColor = Color.Black;
        }

        public void Exibir(EstadoJogo estado)
        {
            _estado = estado;
            Invalidate(); // pede para o painel ser redesenhado
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_estado == null) return;

            Graphics g = e.Graphics;

            // Ajusta o desenho à escala de tela do Windows (100%, 125%, 150%...)
            g.ScaleTransform(
                ClientSize.Width / (float)Jogo.LarguraArea,
                ClientSize.Height / (float)Jogo.AlturaArea);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using var linha = new Pen(Color.DimGray, 3) { DashStyle = DashStyle.Dash };
            g.DrawLine(linha, Jogo.LarguraArea / 2, 0, Jogo.LarguraArea / 2, Jogo.AlturaArea);

            g.FillRectangle(Brushes.DeepSkyBlue, _estado.Raquete1);
            g.FillRectangle(Brushes.Orange, _estado.Raquete2);
            g.FillEllipse(Brushes.White, _estado.Bola);

            string mensagem = _estado.Situacao switch
            {
                SituacaoJogo.Parado => "Clique em Iniciar",
                SituacaoJogo.Pausado => "JOGO PAUSADO",
                SituacaoJogo.Encerrado => $"{_estado.Vencedor} venceu!\nClique em Reiniciar",
                _ => ""
            };

            if (mensagem.Length == 0) return;

            using var fonte = new Font(
                "Segoe UI", 28, FontStyle.Bold, GraphicsUnit.Pixel);
            using var formato = new StringFormat { Alignment = StringAlignment.Center };
            var area = new RectangleF(0, 100, Jogo.LarguraArea, 120);
            g.DrawString(mensagem, fonte, Brushes.Yellow, area, formato);
        }
    }
}
