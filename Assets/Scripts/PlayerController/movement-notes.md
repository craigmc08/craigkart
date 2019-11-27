# car movement
## stuff that needs work
- surface normal detection. it should be smoother on small transitions, but more sharp on faster transitions. the car angle should return to the last surface normal after leaving an obstacle (ramps and kin) and camera should only worry about surface normals (changes due to ramps and kin does not effect camera)
- drifting needs work. and outside drift doesn't exist at all yet.
- reconsider how turboing works, possibly. the slowdown after turboing is instantaneous, which doesn't feel very good.
- wheelie-ing has to exist.
- *when multi-car racing is added* slipstreaming needs to be implemented

## road surface
a raycast can detect when the car is on the ground. the normal of the road surface is noted. this is the car up direction.

## turning
the rotation of the car is updated to have the correct up direction. there is probably a unity built-in for this? as surface normal changes, current forward will be projected onto the new surface plane.

forward direction of the car will be the same as previous forward direction with adjustments for steering. can rotate the previous forward about surface normal 

that is rotation taken car of (except for drifting. ignore that for now :D). although, maybe it will just work?

turning radius is based on current speed and max turning force. can use turning radius + linear speed to find angular speed and adjust forward vector accordingly.
this should be done before computing new quaternion rotation for the kart.

## movement
as surface normal changes, the velocity will be projected onto the new normal plane (for proper slow downs when starting to go up slopes, speed ups when going down, etc.)

rigidbody? not sure. probably best for collisions and bouncing and stuff? but also less control. for now, use rigid body.

have some amount of friction force based on current velocity.

acceleration/deceleration applies a force/acceleration along the car forward axis.

there is extra friction on the right/left car axis to lessen slipping. *note: when drifting, sliding friction (and all friction?) will be reduced significantly*

NEW IDEA: grip force will be based on max turn acceleration. the grip force is enough to perfectly cancel out a turn. when drifting, grip force increases based on tightness for inside drift, but stays the same for outside drift (to allow slipping)

## drifting
HAVE TO SERIOUSLY RETHING OUTSIDE DRIFT

### drift initiation
the hop button is pressed. if on the ground, a small upwards (surface normal) force is applied launching the car into the air.
if the hop button is pressed and there is a steering input with a magnitude greater than some threshold when the car lands, a drift is initiated in the current steering direction.

### drifting mechanics
friction is reduced. mini-turbo charges with a rate based on the magnitude of the steering control in the direction of the drift.

the steering range of the car changes. all the way in the direction is a much tighter turn than normal, neutral steer is still some amount, all the way over is almost straight but a little bit in the drift direction.

