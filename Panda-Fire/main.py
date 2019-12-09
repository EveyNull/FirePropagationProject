from direct.showbase.ShowBase import ShowBase

from ctypes import *

from panda3d.core import CollisionTraverser, CollisionHandlerQueue

from direct.actor.Actor import Actor
from direct.showbase import DirectObject


from fireSystem import fireSystem
from fireManager import fireManager

class MyApp(ShowBase):
 
	def __init__(self):
		ShowBase.__init__(self)
		
		
		base.enableParticles()
		
		base.cTrav = CollisionTraverser("base collision traverser")
		base.handler = CollisionHandlerQueue()
		
		self.scene = self.loader.loadModel("models/environment")
		self.scene.reparentTo(self.render)
		
		self.scene.setScale(0.25, 0.25, 0.25)
		self.scene.setPos(-8,42,0)
		
		self.teapots = []
		self.fireSystems = []
		
		self.fireManager = fireManager()
		
		for i in range(2):	
			self.teapots.append(self.loader.loadModel("teapot"))
			self.teapots[i].reparentTo(render)
			self.teapots[i].setPos(i*15,i,i)
		
		
		for i in range(2,4):	
			self.teapots.append(self.loader.loadModel("teapot"))
			self.teapots[i].reparentTo(render)
			self.teapots[i].setPos((i*15)-30,10,i)
		
		fireSystems = []
		fireSystems.append(fireSystem(0, "greenFire"))
		fireSystems.append(fireSystem(1, "yellowFire"))
		fireSystems.append(fireSystem(2, "purpleFire"))
		fireSystems.append(fireSystem(3, "blueFire"))
		
		for i in range(4):
			fireSystems[i].reparentTo(self.teapots[i])
			fireSystems[i].setPos(3,0,3)
			fireManager.addFireSystem(fireManager, fireSystems[i])

		fireManager.fireSystems[0].ignite()
		fireManager.fireSystems[3].ignite()
		base.disableMouse()
		
		self.tick = taskMgr.add(self.tick, "Update")
		
		self.camera.setPos(0, -20, 5)
		
		
	def tick(self, task):
		newPos = self.teapots[1].getPos()
		newPos[0] -= 3 * globalClock.getDt()
		self.teapots[1].setPos(newPos)
		newPos = self.teapots[3].getPos()
		newPos[0] -= 3 * globalClock.getDt()
		self.teapots[3].setPos(newPos)
		return task.cont
		
app = MyApp()
app.run()