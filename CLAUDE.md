# Fin Tweaks

Plugin próprio do Jellyfin (12.4) para **ajustes pequenos do cliente web**, cada um com um interruptor na página de
admin. Projeto do Angelo, criado na sessão 6 do Jellyfin (2026-10-29). Responda em português do Brasil. O Angelo é
iniciante: explique o porquê das mudanças em uma ou duas frases.

O repositório é **público** (os zips dos releases precisam ser baixáveis pelo Jellyfin). Nada de segredos, IPs,
nomes de usuários ou detalhes da VPS aqui. A operação (Jellyfin, Caddy, backups, PreProbe) está no `~/ops/CLAUDE.md`,
que é a referência; decisões em `~/ops/plano-jellyfin-personalizacao.md` (seção "Sessão 6"). O desenvolvimento é feito
pela sessão **ops** (não há sessão de Remote Control própria). Remoto: `ApenasAngelo/jellyfin-plugin-fin-tweaks`
(público, branch `main`).

## O que entra aqui (e o que não entra)

- **Entra:** ajustes pequenos e independentes do cliente web (JS e, se precisar, CSS), sem lógica de servidor além da
  página de configuração. Ex.: S1 (busca sempre global); candidatos futuros: S3 ("Pastas" na aba Animes), item 2 (nome
  da versão quando há uma só fonte). Lista e numeração no plano do `~/ops`.
- **Não entra:** fileiras e páginas no formato da home (Home Screen Sections Encore), Media Bar e trailers (Media Bar
  Encore), estilo do tema (fork do Abyss, fase 4).

## Desenho combinado

- Um projeto C# mínimo: classe do plugin, `PluginConfiguration` com um booleano por ajuste e a página de admin
  (`configPage.html`) com os interruptores. Configuração só do admin; nenhuma página para o usuário.
- Cada ajuste é um arquivo próprio (`Web/tweaks/<nome>.js`, recurso embutido), servido por um endpoint do plugin.
  O `index.html` recebe, pelo File Transformation, **uma** tag de script que carrega só os ajustes ligados (como o
  Media Bar Enhanced e o Home Screen Sections registram os deles: ver `Helpers/TransformationPatches.cs` e
  `Services/StartupService.cs` no HSS Encore).
- Os scripts não dependem de nomes do código minificado do jellyfin-web: só DOM, rotas (`#/...`) e a API pública.
  O 12.x usa o app **modern** (React) no layout padrão e o **legacy** nos modos `*-legacy`/TV: testar nos dois.
- Navegação do Jellyfin não recarrega a página: reagir a `hashchange`/`popstate` e a mudanças do DOM, com cuidado para
  não criar observadores pesados (a interface precisa continuar rápida, inclusive no app Android, que é WebView).

## Cuidados com o servidor

- **PreProbe do Gelato:** nenhum ajuste pode abrir item por `GET /Items/{id}` ou `/Users/{u}/Items/{id}` nem pedir
  `MediaSources`: isso faz o Gelato analisar a fonte e pôr o torrent no TorBox. Ler itens pela lista
  (`/Items?Ids=...`). Detalhes no `~/ops/CLAUDE.md`.
- Instalar, atualizar ou reiniciar no Jellyfin só com o OK do Angelo, e com backup (`/data/backups/jellyfin/`).
- Capturas: `~/ops/scripts/jellyfin-captura.sh` (conta `claude-teste`).

## Build e publicação

- Build na VPS (ARM64) só em container do .NET 10 SDK (nada de SDK instalado no sistema).
- Release por tag `vX.Y.Z.W` (GitHub Actions) e entrada no manifesto do repositório de plugins
  `ApenasAngelo/jellyfin-plugins`. A montar na sessão 7 (`~/ops/prompts/jellyfin-sessao-7.md`).
- Commits em **inglês** (repositório público, pode receber contribuições), Conventional Commits, atômicos (ex.: `feat(search): always search the whole server`).
  O `CLAUDE.md` e a conversa continuam em português.
