<?php

class Course{
	public function __construct(
		public int $code_course,
		public int $id_professor,
		public string $name_course,
		public string $description_course
	){}
}
?>