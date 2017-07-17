using UnityEngine;
using System.Collections;

namespace SwarmDefender
{
	public class SpaceshipLineController : MonoBehaviour
	{
		public enum ELineType
		{
			MID,
			STRONG
		}

		public Color m_midColor = Color.blue;
		public Color m_strongColor = Color.red;
		public LineRenderer m_lineRenderer1;
		public LineRenderer m_lineRenderer2;
		public LineRenderer m_lineRenderer3;
		public LineRenderer m_lineRenderer4;

		private bool m_isDrawing = false;
		private Transform m_startTransform = null;
		private Transform m_endTransform = null;
		private ELineType m_currentLineType = ELineType.MID;

		private void Start()
		{
			m_lineRenderer1.sortingLayerName = "Lines";
			m_lineRenderer2.sortingLayerName = "Lines";
			m_lineRenderer3.sortingLayerName = "Lines";
			m_lineRenderer4.sortingLayerName = "Lines";
			StopDrawing();
		}

		private void Update()
		{
			if(m_isDrawing)
			{
				UpdateLines();
			}
		}

		public void DrawLines(Transform startTransform, Transform endTransform, ELineType lineType)
		{
			m_isDrawing = true;
			m_startTransform = startTransform;
			m_endTransform = endTransform;
			m_currentLineType = lineType;
			switch(m_currentLineType)
			{
				case ELineType.MID:
					m_lineRenderer1.enabled = true;
					m_lineRenderer2.enabled = true;
					m_lineRenderer3.enabled = true;
					break;
				case ELineType.STRONG:
					m_lineRenderer1.enabled = true;
					m_lineRenderer2.enabled = true;
					m_lineRenderer3.enabled = true;
					m_lineRenderer4.enabled = true;
					break;
			}
		}

		public void StopDrawing()
		{
			m_isDrawing = false;
			m_lineRenderer1.enabled = false;
			m_lineRenderer2.enabled = false;
			m_lineRenderer3.enabled = false;
			m_lineRenderer4.enabled = false;
		}

		private void UpdateLines ()
		{
			if(!m_isDrawing) return;
			if(m_endTransform != null && m_startTransform != null)
			{
				switch(m_currentLineType)
				{
					case ELineType.MID:
						m_lineRenderer1.SetVertexCount(2);
						m_lineRenderer2.SetVertexCount(2);
						m_lineRenderer3.SetVertexCount(2);
						m_lineRenderer1.SetColors(m_midColor, m_midColor);
						m_lineRenderer2.SetColors(m_midColor, m_midColor);
						m_lineRenderer3.SetColors(m_midColor, m_midColor);
						m_lineRenderer1.SetPosition(0, new Vector3(m_startTransform.position.x, m_startTransform.position.y + 7.5f, m_startTransform.position.z));
						m_lineRenderer1.SetPosition(1, new Vector3(m_endTransform.position.x, m_endTransform.position.y + 7.5f, m_endTransform.position.z));
						m_lineRenderer2.SetPosition(0, new Vector3(m_startTransform.position.x, m_startTransform.position.y - 7.5f, m_startTransform.position.z));
						m_lineRenderer2.SetPosition(1, new Vector3(m_endTransform.position.x, m_endTransform.position.y - 7.5f, m_endTransform.position.z));
						m_lineRenderer3.SetPosition(0, new Vector3(m_startTransform.position.x, m_startTransform.position.y + 22.5f, m_startTransform.position.z));
						m_lineRenderer3.SetPosition(1, new Vector3(m_endTransform.position.x, m_endTransform.position.y + 22.5f, m_endTransform.position.z));
					break;
					case ELineType.STRONG:
						m_lineRenderer1.SetVertexCount(2);
						m_lineRenderer2.SetVertexCount(2);
						m_lineRenderer3.SetVertexCount(2);
						m_lineRenderer4.SetVertexCount(2);
						m_lineRenderer1.SetColors(m_strongColor, m_strongColor);
						m_lineRenderer2.SetColors(m_strongColor, m_strongColor);
						m_lineRenderer3.SetColors(m_strongColor, m_strongColor);
						m_lineRenderer4.SetColors(m_strongColor, m_strongColor);
						m_lineRenderer1.SetPosition(0, new Vector3(m_startTransform.position.x, m_startTransform.position.y + 7.5f, m_startTransform.position.z));
						m_lineRenderer1.SetPosition(1, new Vector3(m_endTransform.position.x, m_endTransform.position.y + 7.5f, m_endTransform.position.z));
						m_lineRenderer2.SetPosition(0, new Vector3(m_startTransform.position.x, m_startTransform.position.y - 22.5f, m_startTransform.position.z));
						m_lineRenderer2.SetPosition(1, new Vector3(m_endTransform.position.x, m_endTransform.position.y - 22.5f, m_endTransform.position.z));
						m_lineRenderer3.SetPosition(0, new Vector3(m_startTransform.position.x, m_startTransform.position.y + 37.5f, m_startTransform.position.z));
						m_lineRenderer3.SetPosition(1, new Vector3(m_endTransform.position.x, m_endTransform.position.y + 37.5f, m_endTransform.position.z));
						m_lineRenderer4.SetPosition(0, new Vector3(m_startTransform.position.x, m_startTransform.position.y + 7.5f, m_startTransform.position.z));
						m_lineRenderer4.SetPosition(1, new Vector3(m_endTransform.position.x, m_endTransform.position.y + 7.5f, m_endTransform.position.z));
					break;
				}
			}
			else
			{
				StopDrawing();
			}
		}




	}
}
