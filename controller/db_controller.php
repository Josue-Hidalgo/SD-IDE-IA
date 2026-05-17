<?php 
 function create_db_conn(){// sudo apt-get install php8.4-mysql
 	$servername = "localhost";//cambiar valores por los de la base real
	$username = "root";
	$password = "Qwertys123.";
	$dbname = "prueba";

	$conn =  new mysqli($servername, $username, $password, $dbname);

	if ($conn->connect_error) {
		die("connection failed: ".mysqli_connect_error());
	}
	return $conn;
 }

 //crear profesor

 function create_prof(string $email, string $password, string $name_user, string $lastname_user){
 	$conn = create_db_conn();

 	$sql = "CALL create_user(\"$email\", \"$password\", \"$name_user\", \"$lastname_user\")";
 	if ($conn->query($sql) === TRUE) {
 		echo "se creo el usuario".'<br>';//borrar despues
 		if ($conn->query("CALL create_professor(LAST_INSERT_ID())") === TRUE) {
 			echo "se creo el profesor".'<br>';//borrar despues
 		}else{
 			echo "Error: ".$conn->error;
 		}
 	}else{
 			echo "Error: ".$conn->error;
 		}
 	$conn->close();
 }

 function create_course(string $code_course, int $prof_id, string $name_course, string $desc){
 	$conn = create_db_conn();

 	$sql = "CALL create_course(\"$code_course\", $prof_id, \"$name_course\", \"$desc\")";
 	if ($conn->query($sql) === TRUE) {
 		echo "se creo el curso".'<br>';//borrar despues
 	}else{
 			echo "Error: ".$conn->error;
 		}
 	$conn->close();
 	}

function create_assignment(string $code_course, string $assi_name, string $desc, bool $is_allowed){//agregar la fecha despues
	$conn = create_db_conn();

	$testdate = date("Y-m-d H:i:s");//borrar esto porque genera la fecha en el momento
 	$sql = "CALL create_assignment(\"$code_course\", \"$assi_name\", \"$desc\", \"$testdate\", $is_allowed)";
 	if ($conn->query($sql) === TRUE) {
 		echo "se creo la asignacion".'<br>';//borrar despues
 	}else{
 			echo "Error: ".$conn->error;
 		}
 	$conn->close();
}

function login_user(string $email, string $password){
	$conn = create_db_conn();

 	$sql = "CALL login_user(\"$email\", \"$password\")";
 	$result = $conn->query($sql);
 	if ($result->num_rows >0) {
 		$row = $result->fetch_assoc();
 		$prof_data = [];
 		$prof_data["name"] = $row["name_user"];
 		$prof_data["lastname"] = $row["lastname_user"];
 		$id_user = $row["id_user"];
 		while($conn->more_results()){
 			$conn->next_result();
 			$conn->use_result();
 		}
 		$result2 = $conn->query("SELECT id_professor from Professor where id_user = $id_user");

 		$row2 = $result2->fetch_assoc();
 		$prof_data["prof_id"] = $row2["id_professor"];
 	}else{
 			echo "no result";
 		}
 	$conn->close();
 	return $prof_data;
}

?>