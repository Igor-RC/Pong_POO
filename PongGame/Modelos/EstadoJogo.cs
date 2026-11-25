namespace PongGame
{
    // "Fotografia" imutável da partida. A interface desenha a partir dela,
    // sem acessar os objetos que a thread do jogo está modificando.
    public record EstadoJogo(
        Rectangle Bola,
        Rectangle Raquete1,
        Rectangle Raquete2,
        string Nome1,
        string Nome2,
        int Pontos1,
        int Pontos2,
        SituacaoJogo Situacao,
        string? Vencedor);
}
