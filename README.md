# Pong em C# com Windows Forms e Threads

Jogo Pong para dois jogadores, desenvolvido para a disciplina de Programação Orientada a Objetos.

## Requisitos

- Windows
- .NET 10 SDK
- Visual Studio com a carga de trabalho "Desenvolvimento para desktop com .NET"

## Como executar

Pelo Visual Studio: abra `PongGame.slnx` e pressione **F5**.

Pelo terminal, na pasta raiz do repositório:

```
dotnet run --project PongGame
```

## Controles


 Jogador 1 -> W (cima) e S (baixo) 
 Jogador 2 -> Seta para cima e seta para baixo
 Partida   -> Botões Iniciar, Pausar/Continuar e Reiniciar

## Regras

- A bola rebate nas raquetes e nas bordas superior e inferior.
- Quem deixa a bola passar cede um ponto ao adversário.
- Vence quem fizer 5 pontos primeiro.
- A bola acelera um pouco a cada rebatida, até um limite.

## Organização

|               Arquivo              |                     Responsabilidade                   |
|------------------------------------|--------------------------------------------------------|
| `Modelos/Bola.cs`                  | Posição, velocidade, direção e movimento da bola       |
| `Modelos/Raquete.cs`               | Posição e movimento da raquete, respeitando os limites |
| `Modelos/Jogador.cs`               | Nome, pontuação e raquete do jogador                   |
| `Modelos/Jogo.cs`                  | Regras, colisões, pontuação e andamento da partida     |
| `Modelos/EstadoJogo.cs`            | Cópia imutável da partida enviada à interface          |
| `Modelos/Comandos.cs`              | Teclas de movimento pressionadas                       |
| `Modelos/SituacaoJogo.cs`          | Situações possíveis da partida                         |
| `Threads/MotorJogo.cs`             | Thread do jogo, pausa, encerramento e sincronização    |
| `Interface/FormularioPrincipal.cs` | Janela, botões, placar e teclado                       |
| `Interface/PainelJogo.cs`          | Desenho da área de jogo                                |
| `Interface/BotaoSemFoco.cs`        | Botão que não captura o teclado                        |

## Threads

- `MotorJogo` cria a `ThreadDoJogo`, que atualiza a partida a cada 16 ms, aproximadamente.
- `ManualResetEventSlim` pausa, retoma e encerra a thread.
- `lock` protege os dados compartilhados entre a thread do jogo e a da interface.
- `BeginInvoke` envia cada novo estado para a thread da interface.
- Ao fechar a janela, o encerramento é sinalizado e `Join` aguarda o fim da thread.
