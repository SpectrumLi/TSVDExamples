$base = "C:\Users\t-gual\Documents\OpenSource"
$testdir = $args[0]

$configdir = $base + "\Configurations"
$insconf = $configdir + "\instrumentation-config.cfg"
$runconf = $configdir + "\runtime-config.cfg"
$InsExe = $base+"\TSVD\TSVD\TSVDInstrumenter\bin\Debug\TSVDInstrumenter.exe"


& $InsExe  $testdir $insconf $runconf