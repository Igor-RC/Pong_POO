using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PongGame
{
    // Executa o jogo em uma thread separada da interface gráfica
    public class MotorJogo
    {
        private const int IntervaloMs = 16; // cerca de 60 atualizações por segundo

        private readonly Jogo _jogo;

        // Garante que apenas uma thread por vez acesse o jogo e os comandos
        private readonly object _trava = new object();

        // Sinalizado: a thread executa. Não sinalizado: a thread fica bloqueada (pausa)
        private readonly ManualResetEventSlim _executar = new ManualResetEventSlim(false);

        // Sinalizado quando a thread deve terminar
        private readonly ManualResetEventSlim _encerrar = new ManualResetEventSlim(false);

        private Thread? _thread;
        private Comandos _comandos;
        private bool _encerrado;

        // Disparado dentro da trava: quem assinar NÃO deve bloquear (usar BeginInvoke)
        public event Action<EstadoJogo>? EstadoAtualizado;

        public MotorJogo(Jogo jogo)
        {
            _jogo = jogo;
        }

        public void Iniciar()
        {
            if (_thread == null)
            {
                _thread = new Thread(Executar)
                {
                    Name = "ThreadDoJogo",
                    IsBackground = true
                };
                _thread.Start();
            }

            AlterarJogo(_jogo.Iniciar);
        }

        public void Pausar() => AlterarJogo(_jogo.Pausar);

        public void Retomar() => AlterarJogo(_jogo.Retomar);

        public void Reiniciar() => AlterarJogo(_jogo.Reiniciar);

        public void DefinirComandos(Comandos comandos)
        {
            lock (_trava)
            {
                _comandos = comandos;
            }
        }

        public void PublicarEstado()
        {
            lock (_trava)
            {
                EstadoAtualizado?.Invoke(_jogo.ObterEstado());
            }
        }

        // Encerra a thread e espera ela terminar
        public void Encerrar()
        {
            if (_encerrado) return;
            _encerrado = true;

            lock (_trava)
            {
                _encerrar.Set();
                _executar.Set(); // acorda a thread caso esteja pausada
            }

            _thread?.Join();
            _executar.Dispose();
            _encerrar.Dispose();
        }

        private void AlterarJogo(Action acao)
        {
            lock (_trava)
            {
                acao();

                if (_jogo.Situacao == SituacaoJogo.EmAndamento)
                    _executar.Set();
                else
                    _executar.Reset();

                EstadoAtualizado?.Invoke(_jogo.ObterEstado());
            }
        }

        // Método executado pela thread do jogo
        private void Executar()
        {
            // Melhora a precisão das esperas no Windows (padrão: cerca de 15,6 ms)
            timeBeginPeriod(1);

            var cronometro = new Stopwatch();

            while (true)
            {
                _executar.Wait(); // bloqueia aqui enquanto o jogo não está em andamento
                cronometro.Restart();

                lock (_trava)
                {
                    if (_encerrar.IsSet) break;

                    _jogo.Atualizar(_comandos);

                    if (_jogo.Situacao != SituacaoJogo.EmAndamento)
                        _executar.Reset();

                    EstadoAtualizado?.Invoke(_jogo.ObterEstado());
                }

                int espera = IntervaloMs - (int)cronometro.ElapsedMilliseconds;

                // A espera é interrompida imediatamente se houver pedido de encerramento
                if (espera > 0 && _encerrar.Wait(espera)) break;
            }

            timeEndPeriod(1);
        }

        [DllImport("winmm.dll")]
        private static extern uint timeBeginPeriod(uint periodo);

        [DllImport("winmm.dll")]
        private static extern uint timeEndPeriod(uint periodo);
    }
}
