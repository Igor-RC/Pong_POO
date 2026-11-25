namespace PongGame
{
    // Interface gráfica e interação com o usuário
    public class FormularioPrincipal : Form
    {
        private readonly MotorJogo _motor;
        private readonly Label _lblPlacar;
        private readonly PainelJogo _painel;
        private readonly BotaoSemFoco _btnIniciar;
        private readonly BotaoSemFoco _btnPausar;
        private readonly BotaoSemFoco _btnReiniciar;

        private Comandos _comandos;
        private SituacaoJogo _situacao = SituacaoJogo.Parado;

        public FormularioPrincipal()
        {
            SuspendLayout();

            Text = "Pong";
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(840, 650);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;

            _lblPlacar = new Label
            {
                Location = new Point(20, 10),
                Size = new Size(800, 45),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _painel = new PainelJogo
            {
                Location = new Point(20, 60),
                Size = new Size(Jogo.LarguraArea, Jogo.AlturaArea)
            };

            _btnIniciar = CriarBotao("Iniciar", 205);
            _btnPausar = CriarBotao("Pausar", 355);
            _btnReiniciar = CriarBotao("Reiniciar", 505);

            var lblAjuda = new Label
            {
                Location = new Point(20, 620),
                Size = new Size(800, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Jogador 1: W e S          Jogador 2: setas para cima e para baixo"
            };

            Controls.AddRange(new Control[]
            {
                _lblPlacar, _painel, _btnIniciar, _btnPausar, _btnReiniciar, lblAjuda
            });

            ResumeLayout(false);

            _motor = new MotorJogo(new Jogo());
            _motor.EstadoAtualizado += AoAtualizarEstado;

            _btnIniciar.Click += (s, e) => _motor.Iniciar();
            _btnReiniciar.Click += (s, e) => _motor.Reiniciar();
            _btnPausar.Click += (s, e) =>
            {
                if (_situacao == SituacaoJogo.Pausado)
                    _motor.Retomar();
                else
                    _motor.Pausar();
            };
        }

        private static BotaoSemFoco CriarBotao(string texto, int x)
        {
            return new BotaoSemFoco
            {
                Text = texto,
                Location = new Point(x, 572),
                Size = new Size(130, 40)
            };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _motor.PublicarEstado(); // desenha a tela inicial
        }

        // Pode ser chamado pela thread do jogo: não acessa controles diretamente
        private void AoAtualizarEstado(EstadoJogo estado)
        {
            if (IsDisposed || !IsHandleCreated) return;

            try
            {
                // Envia a atualização para a thread da interface, sem esperar
                BeginInvoke(() => ExibirEstado(estado));
            }
            catch (InvalidOperationException)
            {
                // A janela foi fechada durante o envio: nada a atualizar
            }
        }

        // Executado sempre na thread da interface
        private void ExibirEstado(EstadoJogo estado)
        {
            _situacao = estado.Situacao;

            _lblPlacar.Text =
                $"{estado.Nome1}:  {estado.Pontos1}     x     {estado.Pontos2}  :{estado.Nome2}";

            _btnIniciar.Enabled = _situacao == SituacaoJogo.Parado;
            _btnPausar.Enabled = _situacao == SituacaoJogo.EmAndamento ||
                                 _situacao == SituacaoJogo.Pausado;
            _btnPausar.Text = _situacao == SituacaoJogo.Pausado ? "Continuar" : "Pausar";

            _painel.Exibir(estado);
        }

        // Tecla pressionada (intercepta as setas antes do tratamento padrão do Windows)
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (AtualizarTecla(keyData & Keys.KeyCode, true))
                return true;

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Tecla solta
        protected override void OnKeyUp(KeyEventArgs e)
        {
            AtualizarTecla(e.KeyCode, false);
            base.OnKeyUp(e);
        }

        // Se a janela perder o foco, solta todas as teclas
        protected override void OnDeactivate(EventArgs e)
        {
            _comandos = new Comandos();
            _motor.DefinirComandos(_comandos);
            base.OnDeactivate(e);
        }

        // Guardar o estado das teclas permite que os dois jogadores se movam ao mesmo tempo
        private bool AtualizarTecla(Keys tecla, bool pressionada)
        {
            switch (tecla)
            {
                case Keys.W:
                    _comandos = _comandos with { Jogador1Cima = pressionada };
                    break;
                case Keys.S:
                    _comandos = _comandos with { Jogador1Baixo = pressionada };
                    break;
                case Keys.Up:
                    _comandos = _comandos with { Jogador2Cima = pressionada };
                    break;
                case Keys.Down:
                    _comandos = _comandos with { Jogador2Baixo = pressionada };
                    break;
                default:
                    return false;
            }

            _motor.DefinirComandos(_comandos);
            return true;
        }

        // Encerra a thread do jogo junto com a janela
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _motor.EstadoAtualizado -= AoAtualizarEstado;
            _motor.Encerrar();
            base.OnFormClosed(e);
        }
    }
}
