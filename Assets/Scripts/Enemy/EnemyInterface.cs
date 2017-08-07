using System;

namespace AssemblyCSharp {
	public interface EnemyInterface {
		void idle();
		void walk();
		void search();
		void chase();
		void hunt();
		void kill();
	}
}
