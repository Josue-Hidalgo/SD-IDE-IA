<?php

class Assignment{
	public function __construct(
		public int $id_assignment,
		public string $name_assignment,
		public string $description_assignment,
		public string $deadline,
		public bool $is_allowed_after_deadline
	){}
}
?>