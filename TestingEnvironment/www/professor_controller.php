<?php
include_once 'professor_model.php';

function createProf(int $prof_id,string $email, string $password, string $username, string $userLast){
	$_SESSION['prof_id'] = $prof_id;
	$_SESSION['email'] = $email;
	$_SESSION['password'] = $password;
	$_SESSION['username'] = $username;
	$_SESSION['userLast'] = $userLast;
}

function getProfName(Professor $prof){
	return $_SESSION['username'];
}

function getProfEmail(Professor $prof){
	return $_SESSION['email'];
}

function getProfLName(Professor $prof){
	return $_SESSION['userLast'];
}
function getProfId(Professor $prof){
	return $_SESSION['prof_id'];
}

?>
