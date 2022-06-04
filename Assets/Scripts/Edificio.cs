using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Edificio : MonoBehaviour
{
    bool colocado;
    List <Collider> listaEdificiosColisionados = new List<Collider>();

    void Start ()
    {
        if (GetComponentInChildren<NavMeshLink>())
        {
            StartCoroutine(Colocar());
        }
    }

    void Update ()
    {
        if (listaEdificiosColisionados.Count == 0)
        {
            colocado = true;
        }

        if (transform.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount <= 1f/3f && !GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("En ruinas"))
        {
            gameObject.GetComponentInChildren<Animator>().Play("En ruinas");
        }
        else if (transform.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount <= 2f/3f && !GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Mal estado") && !GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("En ruinas"))
        {
            gameObject.GetComponentInChildren<Animator>().Play("Mal estado");
        }
    }

    IEnumerator Colocar ()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponentInChildren<NavMeshLink>().enabled = true;
    }

    void OnTriggerEnter (Collider otro)
    {
        if (otro.CompareTag("Edificio") && !colocado)
        {
            listaEdificiosColisionados.Add(otro);
        }
    }

    void OnTriggerStay (Collider otro)
    {
        if (otro.CompareTag("Edificio") && !colocado)
        {
            transform.position = transform.position + (transform.position - otro.transform.position) * 0.06f;
        }
    }

    void OnTriggerExit (Collider otro)
    {
        if (otro.CompareTag("Edificio") && !colocado)
        {
            listaEdificiosColisionados.Remove(otro);
        }
    }
}
