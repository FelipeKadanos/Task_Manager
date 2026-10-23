$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Net.Http
$raiz = Split-Path $PSScriptRoot -Parent
$temporario = Join-Path ([IO.Path]::GetTempPath()) ('taskmanager-teste-' + [guid]::NewGuid())
New-Item -ItemType Directory -Path $temporario | Out-Null
$cliente = New-Object System.Net.Http.HttpClient
$cliente.BaseAddress = 'http://localhost:5187'
$processo = $null
$script:verificacoes = 0

function Requisicao($metodo, $rota, $status, $dados = $null) {
    $pedido = New-Object System.Net.Http.HttpRequestMessage($metodo, $rota)
    if ($null -ne $dados) {
        $json = ConvertTo-Json $dados
        $pedido.Content = New-Object System.Net.Http.StringContent($json, [Text.Encoding]::UTF8, 'application/json')
    }
    $resposta = $cliente.SendAsync($pedido).GetAwaiter().GetResult()
    try {
        $texto = $resposta.Content.ReadAsStringAsync().GetAwaiter().GetResult()
        if ([int]$resposta.StatusCode -ne $status) {
            throw "$metodo $rota : esperado $status, recebido $($resposta.StatusCode). $texto"
        }
        $script:verificacoes++
        if ($texto) { return ConvertFrom-Json $texto }
    }
    finally { $resposta.Dispose(); $pedido.Dispose() }
}

Push-Location $raiz
try {
    dotnet build --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'Falha ao compilar.' }
    dotnet ef database update --project TaskManager.Api --no-build --connection "Data Source=$temporario/taskmanager.db;Foreign Keys=True"
    if ($LASTEXITCODE -ne 0) { throw 'Falha ao preparar o banco temporário.' }
    $dll = Join-Path $raiz 'TaskManager.Api/bin/Debug/net8.0/TaskManager.Api.dll'
    $processo = Start-Process dotnet -ArgumentList "`"$dll`" --contentRoot `"$temporario`" --urls http://localhost:5187" -WindowStyle Hidden -PassThru -RedirectStandardOutput "$temporario/api.log" -RedirectStandardError "$temporario/erro.log"
    $pronto = $false
    for ($i = 0; $i -lt 40; $i++) {
        if ($processo.HasExited) { throw (Get-Content "$temporario/erro.log" -Raw) }
        try { $null = Requisicao GET '/' 200; $pronto = $true; break }
        catch { Start-Sleep -Milliseconds 250 }
    }
    if (!$pronto) { throw 'A API não iniciou.' }

    $categoria = Requisicao POST '/categorias' 201 @{ nome = ' Estudos ' }
    $projeto = Requisicao POST '/projetos' 201 @{ nome = ' Projeto ' }
    if ($categoria.nome -ne 'Estudos' -or $projeto.nome -ne 'Projeto') { throw 'Falha na normalização de nomes.' }
    foreach ($rota in @('/categorias', '/projetos')) {
        $null = Requisicao GET $rota 200
        $null = Requisicao GET "$rota/1" 200
        $null = Requisicao PUT "$rota/1" 200 @{ nome = 'Alterado' }
        $null = Requisicao POST $rota 400 @{ nome = ' ' }
        $null = Requisicao GET "$rota/2147483647" 404
    }
    $dados = @{ titulo = ' Tarefa '; categoriaId = $categoria.id }
    $tarefa = Requisicao POST '/tarefas' 201 $dados
    if ($tarefa.status -ne 0 -or $tarefa.prioridade -ne 1 -or $tarefa.atrasada -or $tarefa.titulo -ne 'Tarefa') { throw 'Padrões incorretos.' }
    $dados.projetoId = $projeto.id
    $dados.dataVencimento = '2020-01-01'
    $dados.status = 1
    $dados.prioridade = 2
    $atrasada = Requisicao POST '/tarefas' 201 $dados
    if (!$atrasada.atrasada) { throw 'Atraso incorreto.' }
    $lida = Requisicao GET "/tarefas/$($atrasada.id)" 200
    if ($lida.criadaEmUtc -ne $atrasada.criadaEmUtc) { throw 'Data de criação alterada.' }
    foreach ($filtro in @('status=1', 'prioridade=2', "projetoId=$($projeto.id)", 'atrasadas=true', 'vencimentoDe=2020-01-01&vencimentoAte=2020-01-01', "categoriaId=$($categoria.id)&status=1&prioridade=2&atrasadas=true")) {
        $lista = @(Requisicao GET "/tarefas?$filtro" 200)
        if ($lista.Count -ne 1 -or $lista[0].id -ne $atrasada.id) { throw "Filtro incorreto: $filtro" }
    }
    $lista = @(Requisicao GET '/tarefas?atrasadas=false' 200)
    if ($lista.Count -ne 1 -or $lista[0].id -ne $tarefa.id) { throw 'Filtro de não atrasadas incorreto.' }
    foreach ($filtro in @('status=99', 'prioridade=99', 'categoriaId=0', 'projetoId=-1', 'vencimentoDe=2021-01-01&vencimentoAte=2020-01-01', 'vencimentoDe=invalido')) {
        $null = Requisicao GET "/tarefas?$filtro" 400
    }
    $null = Requisicao POST '/tarefas' 400 @{ titulo = 'Sem categoria' }
    $null = Requisicao POST '/tarefas' 400 @{ titulo = ' '; categoriaId = $categoria.id }
    $null = Requisicao POST '/tarefas' 400 @{ titulo = 'Inválida'; categoriaId = $categoria.id; status = 99 }
    $null = Requisicao POST '/tarefas' 400 @{ titulo = 'Inválida'; categoriaId = $categoria.id; projetoId = 2147483647 }
    $null = Requisicao DELETE "/categorias/$($categoria.id)" 409
    $null = Requisicao DELETE "/projetos/$($projeto.id)" 409
    $null = Requisicao PUT "/tarefas/$($tarefa.id)" 400 @{ titulo = 'Incompleta'; categoriaId = $categoria.id }
    $dados.status = 2
    $dados.projetoId = $null
    $concluida = Requisicao PUT "/tarefas/$($atrasada.id)" 200 $dados
    if ($concluida.atrasada -or $concluida.criadaEmUtc -ne $atrasada.criadaEmUtc) { throw 'Conclusão incorreta.' }
    $null = Requisicao DELETE "/projetos/$($projeto.id)" 204
    foreach ($id in @($tarefa.id, $atrasada.id)) {
        $null = Requisicao DELETE "/tarefas/$id" 204
        $null = Requisicao GET "/tarefas/$id" 404
    }
    $null = Requisicao DELETE "/categorias/$($categoria.id)" 204
    Write-Host "$script:verificacoes requisições verificadas com sucesso."
}
finally {
    if ($processo -and !$processo.HasExited) { Stop-Process -Id $processo.Id; $processo.WaitForExit() }
    $cliente.Dispose()
    Pop-Location
    # O diretório exclusivo fica disponível para inspecionar o banco e os logs.
    Write-Host "Banco e logs de teste: $temporario"
}
