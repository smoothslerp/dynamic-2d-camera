public struct CameraLimits {
		public float leftLimit;
		public float rightLimit;
		public float upLimit;
		public float downLimit;

		public CameraLimits(float l, float r, float u, float d) {
			this.leftLimit = l;
			this.rightLimit = r;
			this.upLimit = u;
			this.downLimit = d;
		}

		// copy
		public CameraLimits(CameraLimits c) {
			this.leftLimit = c.leftLimit;
			this.rightLimit = c.rightLimit;
			this.upLimit = c.upLimit;
			this.downLimit = c.downLimit;
		}

        public override string ToString() {
            return "left: " + this.leftLimit + " right: " + this.rightLimit + " up: " + this.upLimit + " down: " + this.downLimit;
        }
	}