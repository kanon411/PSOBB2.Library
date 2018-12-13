using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RootMotion.FinalIK
{

	/// <summary>
	/// Component that is similar to VR-IK component
	/// that contains references to bones that can be automatically initialized.
	/// </summary>
	public sealed class AvatarBoneSDKData : MonoBehaviour
	{
		/// <summary>
		/// VRIK-specific definition of a humanoid biped.
		/// </summary>
		[System.Serializable]
		public class References
		{
			public Transform root;          // 0
			public Transform pelvis;        // 1
			public Transform spine;         // 2

			[Tooltip("Optional")]
			public Transform chest;         // 3 Optional

			[Tooltip("Optional")]
			public Transform neck;          // 4 Optional
			public Transform head;          // 5

			[Tooltip("Optional")]
			public Transform leftShoulder;  // 6 Optional
			public Transform leftUpperArm;  // 7
			public Transform leftForearm;   // 8
			public Transform leftHand;      // 9

			[Tooltip("Optional")]
			public Transform rightShoulder; // 10 Optional
			public Transform rightUpperArm; // 11
			public Transform rightForearm;  // 12
			public Transform rightHand;     // 13

			[Tooltip("VRIK also supports legless characters.If you do not wish to use legs, leave all leg references empty.")]
			public Transform leftThigh;     // 14 Optional

			[Tooltip("VRIK also supports legless characters.If you do not wish to use legs, leave all leg references empty.")]
			public Transform leftCalf;      // 15 Optional

			[Tooltip("VRIK also supports legless characters.If you do not wish to use legs, leave all leg references empty.")]
			public Transform leftFoot;      // 16 Optional

			[Tooltip("Optional")]
			public Transform leftToes;      // 17 Optional

			[Tooltip("VRIK also supports legless characters.If you do not wish to use legs, leave all leg references empty.")]
			public Transform rightThigh;    // 18 Optional

			[Tooltip("VRIK also supports legless characters.If you do not wish to use legs, leave all leg references empty.")]
			public Transform rightCalf;     // 19 Optional

			[Tooltip("VRIK also supports legless characters.If you do not wish to use legs, leave all leg references empty.")]
			public Transform rightFoot;     // 20 Optional

			[Tooltip("Optional")]
			public Transform rightToes;     // 21 Optional

			/// <summary>
			/// Returns an array of all the Transforms in the definition.
			/// </summary>
			public Transform[] GetTransforms()
			{
				return new Transform[22] {
					root, pelvis, spine, chest, neck, head, leftShoulder, leftUpperArm, leftForearm, leftHand, rightShoulder, rightUpperArm, rightForearm, rightHand, leftThigh, leftCalf, leftFoot, leftToes, rightThigh, rightCalf, rightFoot, rightToes
				};
			}

			/// <summary>
			/// Returns true if all required Transforms have been assigned (shoulder, toe and neck bones are optional).
			/// </summary>
			public bool isFilled
			{
				get
				{
					if(
						root == null ||
						pelvis == null ||
						spine == null ||
						head == null ||
						leftUpperArm == null ||
						leftForearm == null ||
						leftHand == null ||
						rightUpperArm == null ||
						rightForearm == null ||
						rightHand == null
					) return false;

					// If all leg bones are null, it is valid
					bool noLegBones =
						leftThigh == null &&
						leftCalf == null &&
						leftFoot == null &&
						rightThigh == null &&
						rightCalf == null &&
						rightFoot == null;

					if(noLegBones) return true;

					bool atLeastOneLegBoneMissing =
						leftThigh == null ||
						leftCalf == null ||
						leftFoot == null ||
						rightThigh == null ||
						rightCalf == null ||
						rightFoot == null;

					if(atLeastOneLegBoneMissing) return false;

					// Shoulder, toe and neck bones are optional
					return true;
				}
			}

			/// <summary>
			/// Returns true if none of the Transforms have been assigned.
			/// </summary>
			public bool isEmpty
			{
				get
				{
					if(
						root != null ||
						pelvis != null ||
						spine != null ||
						chest != null ||
						neck != null ||
						head != null ||
						leftShoulder != null ||
						leftUpperArm != null ||
						leftForearm != null ||
						leftHand != null ||
						rightShoulder != null ||
						rightUpperArm != null ||
						rightForearm != null ||
						rightHand != null ||
						leftThigh != null ||
						leftCalf != null ||
						leftFoot != null ||
						leftToes != null ||
						rightThigh != null ||
						rightCalf != null ||
						rightFoot != null ||
						rightToes != null
					) return false;

					return true;
				}
			}

			/// <summary>
			/// Auto-detects Avatar transform references. Works with a Humanoid Animator on the root gameobject only.
			/// </summary>
			public static bool AutoDetectReferences(Transform root, out References references)
			{
				references = new References();

				var animator = root.GetComponentInChildren<Animator>();
				if(animator == null || !animator.isHuman)
				{
					Debug.LogWarning("Avatar needs a Humanoid Animator to auto-detect biped references. Please assign references manually.");
					return false;
				}

				references.root = root;
				references.pelvis = animator.GetBoneTransform(HumanBodyBones.Hips);
				references.spine = animator.GetBoneTransform(HumanBodyBones.Spine);
				references.chest = animator.GetBoneTransform(HumanBodyBones.Chest);
				references.neck = animator.GetBoneTransform(HumanBodyBones.Neck);
				references.head = animator.GetBoneTransform(HumanBodyBones.Head);
				references.leftShoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
				references.leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
				references.leftForearm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
				references.leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
				references.rightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
				references.rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
				references.rightForearm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
				references.rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
				references.leftThigh = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
				references.leftCalf = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
				references.leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
				references.leftToes = animator.GetBoneTransform(HumanBodyBones.LeftToes);
				references.rightThigh = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
				references.rightCalf = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
				references.rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
				references.rightToes = animator.GetBoneTransform(HumanBodyBones.RightToes);

				return true;
			}
		}

		/// <summary>
		/// Bone mapping. Right-click on the component header and select 'Auto-detect References' of fill in manually if not a Humanoid character. Chest, neck, shoulder and toe bones are optional. VRIK also supports legless characters. If you do not wish to use legs, leave all leg references empty.
		/// </summary>
		[ContextMenuItem("Auto-detect References", "AutoDetectReferences")]
		[Tooltip("Bone mapping. Right-click on the component header and select 'Auto-detect References' of fill in manually if not a Humanoid character. Chest, neck, shoulder and toe bones are optional. Also supports legless characters. If you do not wish to use legs, leave all leg references empty.")]
		public References references = new References();

		/// <summary>
		/// Auto-detects bone references for this Avatar. Works with a Humanoid Animator on the gameobject only.
		/// </summary>
		[ContextMenu("Auto-detect References")]
		public void AutoDetectReferences()
		{
			References.AutoDetectReferences(transform, out references);
		}
	}
}