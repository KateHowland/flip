/*
 * Created by SharpDevelop.
 * User: kn70
 * Date: 01/12/2010
 * Time: 11:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
	/// <summary>
	/// Description of ScriptHelper.
	/// </summary>
	public enum Attachment
	{
		Ignore, // all scripts
		Attached, // only scripts which are attached to a conversation or script slot
		AttachedToConversation, // only scripts attached to a conversation
		AttachedToScriptSlot // only scripts attached to a script slot
	}
}