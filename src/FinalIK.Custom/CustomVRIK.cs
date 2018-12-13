using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FinalIK;
using Guardians.FinalIK;

namespace RootMotion.FinalIK
{

	/// <summary>
	/// A full-body IK solver designed specifically for a VR HMD and hand controllers.
	/// </summary>
	//[HelpURL("http://www.root-motion.com/finalikdox/html/page7.html")]
	[AddComponentMenu("Scripts/RootMotion.FinalIK/IK/VR IK")]
	public class CustomVRIK : IK
	{
		// Open the User Manual URL
		[ContextMenu("User Manual")]
		protected override void OpenUserManual()
		{
			Debug.Log("Sorry, VRIK User Manual is not finished yet.");
			// TODO Application.OpenURL("http://www.root-motion.com/finalikdox/html/page6.html");
		}

		// Open the Script Reference URL
		[ContextMenu("Scrpt Reference")]
		protected override void OpenScriptReference()
		{
			Debug.Log("Sorry, VRIK Script reference is not finished yet.");
			// TODO Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_full_body_biped_i_k.html");
		}

		// Open a video tutorial about setting up the component
		[ContextMenu("TUTORIAL VIDEO (STEAMVR SETUP)")]
		void OpenSetupTutorial()
		{
			Application.OpenURL("https://www.youtube.com/watch?v=6Pfx7lYQiIA&feature=youtu.be");
		}

		/// <summary>
		/// Bone mapping. Right-click on the component header and select 'Auto-detect References' of fill in manually if not a Humanoid character. Chest, neck, shoulder and toe bones are optional. VRIK also supports legless characters. If you do not wish to use legs, leave all leg references empty.
		/// </summary>
		[ContextMenuItem("Auto-detect References", "AutoDetectReferences")]
		[Tooltip("Bone mapping. Right-click on the component header and select 'Auto-detect References' of fill in manually if not a Humanoid character. Chest, neck, shoulder and toe bones are optional. VRIK also supports legless characters. If you do not wish to use legs, leave all leg references empty.")]
		public CustomVRIKReferences references = new CustomVRIKReferences();

		/// <summary>
		/// The solver.
		/// </summary>
		[Tooltip("The VRIK solver.")]
		public IKSolverVR solver = new IKSolverVR();

		/// <summary>
		/// Auto-detects bone references for this VRIK. Works with a Humanoid Animator on the gameobject only.
		/// </summary>
		[ContextMenu("Auto-detect References")]
		public void AutoDetectReferences()
		{
			CustomVRIKReferences.AutoDetectReferences(transform, out references);
		}

		/// <summary>
		/// Fills in arm wristToPalmAxis and palmToThumbAxis.
		/// </summary>
		[ContextMenu("Guess Hand Orientations")]
		public void GuessHandOrientations()
		{
			solver.GuessHandOrientations(references, false);
		}

		public override IKSolver GetIKSolver()
		{
			return solver as IKSolver;
		}

		protected override void InitiateSolver()
		{
			if(references.isEmpty) AutoDetectReferences();
			if(references.isFilled) solver.SetToReferences(references);

			base.InitiateSolver();
		}

		/// <summary>
		/// Reinitializes the IK and Solver.
		/// </summary>
		public void ReInitialize()
		{
			solver.initiated = false;
			componentInitiated = false;
			FindAnimatorRecursive(transform, true);
			
			//We only call the base solver because on reinit we will want to assume
			//that the avatar has been replaced, and we'll need to find the data component that can be used to retreieve it.
			IAvatarIKReferenceContainer<CustomVRIKReferences> referenceContainer = this.GetComponentInChildren<IAvatarIKReferenceContainer<CustomVRIKReferences>>();

			//If we contain no reference data
			//then we should just autodetect and wish for the best
			//though this could cost performance issues on failure maybe?
			if(referenceContainer == null)
				AutoDetectReferences();
			else
			{
				//Set the root as the object this is attached to
				referenceContainer.references.SetRoot(this.transform);
				solver.SetToReferences(referenceContainer.references);
			}

			base.InitiateSolver();
			componentInitiated = true;
		}

		protected override void UpdateSolver()
		{
			if(references.root != null && references.root.localScale == Vector3.zero)
			{
				Debug.LogError("VRIK Root Transform's scale is zero, can not update VRIK. Make sure you have not calibrated the character to a zero scale.", transform);
				enabled = false;
				return;
			}

			base.UpdateSolver();
		}
	}
}
