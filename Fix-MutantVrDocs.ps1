$root = Join-Path (Get-Location).Path "Packages/com.mutant.vr/Documentation~"

if (-not (Test-Path $root)) {
    New-Item -ItemType Directory -Path $root -Force | Out-Null
}

$utf8Bom = [System.Text.UTF8Encoding]::new($true)

[System.IO.File]::WriteAllText(
    (Join-Path $root "com.mutant.vr-overview.md"),
@"
# Mutant VR Overview

第一版包结构总览文档。
"@,
    $utf8Bom
)

[System.IO.File]::WriteAllText(
    (Join-Path $root "com.mutant.vr-quickstart.md"),
@"
# Mutant VR Quick Start

后续补充快速接入步骤。
"@,
    $utf8Bom
)

[System.IO.File]::WriteAllText(
    (Join-Path $root "com.mutant.vr-vrif-integration.md"),
@"
# Mutant VR VRIF Integration

后续补充 VRIF 适配说明。
"@,
    $utf8Bom
)

[System.IO.File]::WriteAllText(
    (Join-Path $root "com.mutant.vr-module-lifecycle.md"),
@"
# Mutant VR Module Lifecycle

后续补充模块生命周期说明。
"@,
    $utf8Bom
)

[System.IO.File]::WriteAllText(
    (Join-Path $root "com.mutant.vr-sample-setup.md"),
@"
# Mutant VR Sample Setup

后续补充 Sample 场景搭建说明。
"@,
    $utf8Bom
)

Write-Host "Documentation~ repaired." -ForegroundColor Green