
class fireManager():
	
	fireSystems = []
	
	def __init__(self):
		self.update = base.taskMgr.add(self.update, "FireManager")
		
	def addFireSystem(self, fireSystem):
		self.fireSystems.append(fireSystem)
		
		
	def update(self, task):
		if base.handler.getNumEntries() > 0:
			for entry in base.handler.getEntries():
				i = str(entry.getIntoNodePath().getParent()).split('/')
				j = str(entry.getFromNodePath().getParent()).split('/')
				if "fire" in i[len(i)-1]:
					a = int(i[len(i)-1][-1:])
					b = int(j[len(j)-1][-1:])
					if a < len(self.fireSystems) and b < len(self.fireSystems):
						if self.fireSystems[a].isAlight and not self.fireSystems[b].isAlight:
							self.fireSystems[b].ignite()
		return task.cont
	