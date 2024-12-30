using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    protected Animator anim;
    private float zoomAnimTime;
    public float ZoomAnimTime => zoomAnimTime;

    // Đánh dấu phương thức Start là protected để có thể truy cập từ lớp con
    protected void Start()
    {
        anim = GetComponent<Animator>();
        SetZoomAnimTime();
    }

    private void SetZoomAnimTime()
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "ZoomIn Anim")
            {
                zoomAnimTime = clip.length;
                return;
            }
        }
    }

    // Đánh dấu phương thức ZoomIn là virtual để có thể ghi đè
    public virtual void ZoomIn(bool zoomIn)
    {
        anim.SetBool("ZoomIn", zoomIn);
    }
}