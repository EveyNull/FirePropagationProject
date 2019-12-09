from direct.showbase.ShowBase import ShowBase
from math import pi, sin, cos
from panda3d.core import Filename
from panda3d.core import CollisionSphere, CollisionTraverser, CollisionNode
from panda3d.core import CollisionHandlerQueue, CollisionHandlerEvent, CollisionRay
from panda3d.core import TextNode
from panda3d.physics import BaseParticleEmitter, BaseParticleRenderer
from panda3d.physics import PointParticleFactory, SpriteParticleRenderer
from panda3d.physics import LinearNoiseForce, DiscEmitter
from direct.actor.Actor import Actor

from direct.gui.OnscreenText import OnscreenText
from direct.particles.Particles import Particles
from direct.particles.ParticleEffect import ParticleEffect

import logging
 
HIGHLIGHT = (0, 1, 1, 1)

class fireSystem():

	lifeSpanSeconds = 20
	lifeRemaining = 0
	
	isAlight = False

	def __init__(self, id, parent):
		self.id = id
		self.actor = Actor()
		self.actor.reparentTo(parent)
		
		self.lifeRemaining = self.lifeSpanSeconds
		
		self.fire = ParticleEffect()
		self.fire.loadConfig('fire.ptf')
		self.fire.reparentTo(self.actor)
		self.fire.setPos(0.000, 0.000, 4.250)
		
		fireSphere = CollisionSphere(0,0,4.25,2.5)
		self.collisionNodeName = "fire{}Collision".format(self.id)
		fireCollisionNode = CollisionNode(self.collisionNodeName)
		fireCollisionNode.addSolid(fireSphere)
		self.collision = self.actor.attachNewNode(fireCollisionNode)
		base.cTrav.addCollider(self.collision, base.handler)
		
		self.burn = taskMgr.add(self.burnTask, "burnTask")
		
		self.notifier = CollisionHandlerEvent
		
	def burnTask(self, task):
		if self.isAlight:
			
			deltaTime = globalClock.getDt()
			self.lifeRemaining -= deltaTime
			if self.lifeRemaining <= 0:
				self.fire.softStop()
			else:
				return task.cont
		else:
			return task.cont
			
	def ignite(self):
		self.isAlight = True
		self.fire.start()
		
		
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
		
		self.teapots = [None for i in range(2)]
		self.fires = [None for i in range(2)]
		
		for i in range(2):			
			self.teapots[i] = self.loader.loadModel("teapot")
			self.teapots[i].reparentTo(render)
			self.fires[i] = fireSystem(i, self.teapots[i])
			self.teapots[i].setPos(i*15,i,i)
		
		self.fires[1].ignite()
		self.tick = taskMgr.add(self.tick, "Update")
		
		base.disableMouse()
		self.camera.setPos(0, -20, 5)
		
	def tick(self, task):
		if self.handler.getNumEntries() > 0:
			for entry in self.handler.getEntries():
				i = int(str(entry.getIntoNodePath().getParent().getChild(0))[-1:])
				if not self.fires[i-1].isAlight:
					self.fires[i-1].ignite()
		
		newPos = self.teapots[1].getPos()
		newPos[0] -= 1 * globalClock.getDt()
		self.teapots[1].setPos(newPos)
		return task.cont
		
app = MyApp()
app.run()