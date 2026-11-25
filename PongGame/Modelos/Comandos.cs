namespace PongGame
{
    // Teclas de movimento que estão pressionadas em um dado instante
    public readonly record struct Comandos(
        bool Jogador1Cima,
        bool Jogador1Baixo,
        bool Jogador2Cima,
        bool Jogador2Baixo);
}
