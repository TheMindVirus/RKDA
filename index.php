<?php $addr = gethostbyname($_SERVER["__FILE__"]); $files = scandir(".");
echo "<title>RKDΛ</title><!--Specialising in 3D Games for Embedded Systems-->";
echo "<style>* { background: black; font-family: sans-serif; font-size: 20pt; } h1 { color: blue; font-size: 40pt; }</style>";
echo '<img src="RKDΛ_LOGO.PNG" alt="[RKDΛ]" width="200"/></br>';
foreach ($files as $key => $file)
{
    if ((is_dir($file)) && ($file != ".") && ($file != "..") && ($file != "@eaDir")) { echo '<a href="' . $file . '">' . $file . '</a></br>'; }
}
echo '<img src="RKDΛ_FONT.PNG" alt="[RKDΛ]"/></br>';
exit; ?>