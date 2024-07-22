class Log:
	INFO = u"\u001b[36m"
	WARNING = u"\u001b[31m"
	ERROR = u"\u001b[33m"
	RESET = u"\u001b[0m"

	@classmethod
	def info(cls, message):
		print(f"{Log.INFO}[VALPORTING] {Log.RESET}{message}")

	@classmethod
	def warn(cls, message):
		print(f"{Log.WARNING}[VALPORTING] {Log.RESET}{message}")
		
	@classmethod
	def error(cls, message):
		print(f"{Log.ERROR}[VALPORTING] {Log.RESET}{message}")