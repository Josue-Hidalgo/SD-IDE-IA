<?php
	class professor{
		public function __construct(
			public int $id_professor,
			public string $email,
			public string $password,
			public string $name_user,
			public string $lastname_user
		){}
	}
?>