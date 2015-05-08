using UnityEngine;
using System.Collections;
using System.Linq;

namespace MyUtilities {

	public static class GameObjectUtil {

		public enum RecurseType {
			RECURSE,
			NO_RECURSE
		};

		public delegate void Handler(GameObject child);

		/// <summary>
		/// Launch method to iterate the children of <see cref="parent"/> object, applying the
		/// <see cref="handler"/> method to each child.  Does so recursively if 
		/// <see cref="recurseType"/> is set to <see cref="RECURSE"/>
		/// </summary>
		/// <param name="parent">Parent game object.</param>
		/// <param name="handler">Handler method.</param>
		/// <param name="recurseType">Recurse type.</param>
		public static void IterateChildren(GameObject parent, Handler handler, RecurseType recurseType) {
			Iterate(parent, handler, recurseType);
		}

		/// <summary>
		/// Potentially recursive method to apply a handler method to children.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="handler">Handler.</param>
		/// <param name="recurseType">Recurse type.</param>
		private static void Iterate (GameObject parent, Handler handler, RecurseType recurseType)
		{
			foreach(Transform child in parent.transform) {
				handler(child.gameObject);
				if (recurseType == RecurseType.RECURSE) {
					Iterate(child.gameObject, handler, recurseType);
				}
			}
		}
	}
}

