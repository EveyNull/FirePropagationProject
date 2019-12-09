from math import pi, sin, cos, floor
from panda3d.core import Filename
from panda3d.core import CollisionSphere, CollisionNode, CollisionHandlerEvent
from panda3d.core import LColor
from panda3d.core import NodePath
from panda3d.physics import BaseParticleEmitter, BaseParticleRenderer
from panda3d.physics import PointParticleFactory, SpriteParticleRenderer
from panda3d.physics import LinearNoiseForce, DiscEmitter
from direct.particles.Particles import Particles
from direct.particles.ParticleEffect import ParticleEffect

class fireSystem(NodePath):

	lifeSpanSeconds = 1
	lifeRemaining = 0
	
	isAlight = False

	def __init__(self, name, fireType):
		NodePath.__init__(self, "fire" + str(name))
		NodePath.setPythonTag(self, "fireSystem", self)
		self.setScale(1,1,1)
		
		self.fire = ParticleEffect()
		self.fire.loadConfig('fire.ptf')
		try:
			file = open(fireType + ".txt")
			lines = file.readlines()
			newFireColor = LColor()
			colorLine = lines[0].split(",")
			for i in range(4):
				newFireColor[i] = float(colorLine[i])
			self.fire.getParticlesList()[0].getRenderer().setColor(newFireColor)
			self.lifeSpanSeconds = float(lines[1])
			file.close()
		except IOError:
			print("Firetype not found")
			
			
		self.lifeRemaining = self.lifeSpanSeconds
		fireSphere = CollisionSphere(0,0,1.25,2.5)
		self.collisionNodeName = "fire{}Collision".format(self.id)
		fireCollisionNode = CollisionNode(self.collisionNodeName)
		fireCollisionNode.addSolid(fireSphere)
		self.collision = self.attachNewNode(fireCollisionNode)
		base.cTrav.addCollider(self.collision, base.handler)
		
		self.fire.start(self, self)
		self.fire.softStop()
		self.burn = taskMgr.add(self.burnTask, "burnTask")
		
		self.notifier = CollisionHandlerEvent
		
	def burnTask(self, task):
		if self.isAlight:
			deltaTime = globalClock.getDt()
			self.lifeRemaining -= deltaTime
			if self.lifeRemaining <= 0:
				self.fire.softStop()
				self.isAlight = False
		return task.cont
			
	def ignite(self):
		self.isAlight = True
		self.lifeRemaining = self.lifeSpanSeconds
		self.fire.softStart()
		
		