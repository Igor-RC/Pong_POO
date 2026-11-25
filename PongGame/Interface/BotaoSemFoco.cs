namespace PongGame
{
    // Botão que não recebe foco, para não interferir no uso do teclado durante o jogo
    public class BotaoSemFoco : Button
    {
        public BotaoSemFoco()
        {
            SetStyle(ControlStyles.Selectable, false);
        }
    }
}
