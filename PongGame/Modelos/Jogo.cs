namespace PongGame
{
    // Controla as regras, as colisões, a pontuação e o andamento da partida
    public class Jogo
    {
        public const int LarguraArea = 800;
        public const int AlturaArea = 500;
        public const int PontuacaoVitoria = 5;

        private const int MargemRaquete = 20;
        private const int LarguraRaquete = 12;
        private const int AlturaRaquete = 90;
        private const int VelocidadeRaquete = 8;
        private const int QuadrosAntesDoSaque = 45; // cerca de 0,75 s

        private readonly Bola _bola;
        private readonly Jogador _jogador1;
        private readonly Jogador _jogador2;
        private Jogador? _vencedor;
        private int _esperaSaque;

        public SituacaoJogo Situacao { get; private set; }

        public Jogo()
        {
            var raquete1 = new Raquete(MargemRaquete, 0,
                LarguraRaquete, AlturaRaquete, VelocidadeRaquete);

            var raquete2 = new Raquete(LarguraArea - MargemRaquete - LarguraRaquete, 0,
                LarguraRaquete, AlturaRaquete, VelocidadeRaquete);

            _jogador1 = new Jogador("Jogador 1", raquete1);
            _jogador2 = new Jogador("Jogador 2", raquete2);
            _bola = new Bola(14, 6, 12);

            Reiniciar();
        }

        public void Iniciar()
        {
            if (Situacao != SituacaoJogo.Parado) return;

            _esperaSaque = QuadrosAntesDoSaque;
            Situacao = SituacaoJogo.EmAndamento;
        }

        public void Pausar()
        {
            if (Situacao == SituacaoJogo.EmAndamento)
                Situacao = SituacaoJogo.Pausado;
        }

        public void Retomar()
        {
            if (Situacao == SituacaoJogo.Pausado)
                Situacao = SituacaoJogo.EmAndamento;
        }

        public void Reiniciar()
        {
            _jogador1.ZerarPontuacao();
            _jogador2.ZerarPontuacao();
            _jogador1.Raquete.Centralizar(AlturaArea);
            _jogador2.Raquete.Centralizar(AlturaArea);
            _bola.Centralizar(LarguraArea, AlturaArea);
            _vencedor = null;
            Situacao = SituacaoJogo.Parado;
        }

        // Avança um passo (quadro) da partida
        public void Atualizar(Comandos comandos)
        {
            if (Situacao != SituacaoJogo.EmAndamento) return;

            MoverRaquete(_jogador1.Raquete, comandos.Jogador1Cima, comandos.Jogador1Baixo);
            MoverRaquete(_jogador2.Raquete, comandos.Jogador2Cima, comandos.Jogador2Baixo);

            // As raquetes se movem, mas a bola aguarda no centro antes do saque
            if (_esperaSaque > 0)
            {
                _esperaSaque--;
                return;
            }

            _bola.Mover();
            VerificarColisaoRaquetes();
            VerificarColisaoBordas();
            VerificarPonto();
        }

        public EstadoJogo ObterEstado()
        {
            return new EstadoJogo(
                _bola.Limites,
                _jogador1.Raquete.Limites,
                _jogador2.Raquete.Limites,
                _jogador1.Nome,
                _jogador2.Nome,
                _jogador1.Pontuacao,
                _jogador2.Pontuacao,
                Situacao,
                _vencedor?.Nome);
        }

        private static void MoverRaquete(Raquete raquete, bool cima, bool baixo)
        {
            if (cima) raquete.MoverParaCima();
            if (baixo) raquete.MoverParaBaixo(AlturaArea);
        }

        private void VerificarColisaoRaquetes()
        {
            if (_bola.VelocidadeX < 0)
            {
                // Face direita da raquete esquerda
                double face = _jogador1.Raquete.Limites.Right;

                if (_bola.XAnterior >= face && _bola.X <= face &&
                    AlcancaRaquete(_jogador1.Raquete, face))
                {
                    _bola.RebaterHorizontal(face);
                }
            }
            else
            {
                // Posição X da bola quando encosta na raquete direita
                double face = _jogador2.Raquete.Limites.Left - _bola.Tamanho;

                if (_bola.XAnterior <= face && _bola.X >= face &&
                    AlcancaRaquete(_jogador2.Raquete, face))
                {
                    _bola.RebaterHorizontal(face);
                }
            }
        }

        // Calcula a altura da bola no instante em que cruzou a face da raquete.
        // Assim a bola não atravessa a raquete, mesmo em alta velocidade.
        private bool AlcancaRaquete(Raquete raquete, double xContato)
        {
            double fracao = (xContato - _bola.XAnterior) / (_bola.X - _bola.XAnterior);
            double yContato = _bola.YAnterior + (_bola.Y - _bola.YAnterior) * fracao;

            return yContato + _bola.Tamanho >= raquete.Limites.Top &&
                   yContato <= raquete.Limites.Bottom;
        }

        private void VerificarColisaoBordas()
        {
            double limiteInferior = AlturaArea - _bola.Tamanho;

            if (_bola.Y <= 0 && _bola.VelocidadeY < 0)
                _bola.RebaterVertical(0);
            else if (_bola.Y >= limiteInferior && _bola.VelocidadeY > 0)
                _bola.RebaterVertical(limiteInferior);
        }

        private void VerificarPonto()
        {
            if (_bola.X + _bola.Tamanho < 0)
                RegistrarPonto(_jogador2);
            else if (_bola.X > LarguraArea)
                RegistrarPonto(_jogador1);
        }

        private void RegistrarPonto(Jogador jogador)
        {
            jogador.MarcarPonto();
            _bola.Centralizar(LarguraArea, AlturaArea);

            if (jogador.Pontuacao >= PontuacaoVitoria)
            {
                _vencedor = jogador;
                Situacao = SituacaoJogo.Encerrado;
            }
            else
            {
                _esperaSaque = QuadrosAntesDoSaque;
            }
        }
    }
}
