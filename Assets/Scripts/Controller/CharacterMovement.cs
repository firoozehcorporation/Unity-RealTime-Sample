using UnityEngine;

namespace Controller
{
    public class CharacterMovement : MonoBehaviour {

        [Header("Movement Attribute")]
        public float speed = 5;
        public float rotAngle = 0;
        public float turnSmoothTime = 0.2f;
        public Vector3 forward;
        float turnnSmoothVelocity;
        [Header("Reference")]
        CharacterController controller;
        public float inputHorizontal;
        public float inputVertical;
    
        public GameObject m_inpul_left;
        public GameObject m_inpul_Right;

        [Header("Platform")]
        public bool PC = true;
        // Use this for initialization
        void Start () {
            controller = GetComponent<CharacterController>();
        }
	
        // Update is called once per frame
        void Update () {
            inputHorizontal = Input.GetAxisRaw("Vertical");

            if(!PC)
                controller.Move(Vector3.forward * (m_inpul_left.GetComponent<MobileInputController>().Vertical * Time.deltaTime * speed)+ Vector3.right * (m_inpul_left.GetComponent<MobileInputController>().Horizontal * Time.deltaTime * speed));
            else
                controller.Move(Vector3.forward * (Input.GetAxisRaw("Vertical") * Time.deltaTime * speed) + Vector3.right * (Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed));

            var from = new Vector3(0f,0f,1f);
            var to = new Vector3(m_inpul_Right.GetComponent<MobileInputController>().Horizontal,0f, m_inpul_Right.GetComponent<MobileInputController>().Vertical);


            if (m_inpul_Right.GetComponent<MobileInputController>().Horizontal == 0 ||
                m_inpul_Right.GetComponent<MobileInputController>().Vertical == 0) return;
            var angle = Vector3.SignedAngle(from, to, Vector3.up);
            rotAngle = angle;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnnSmoothVelocity, turnSmoothTime);

        }

    }
}
