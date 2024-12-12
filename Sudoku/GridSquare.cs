using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using static GameEvents;
using System.Xml.Serialization;
using Unity.VisualScripting;
public class GridSquare : Selectable, IPointerClickHandler, ISubmitHandler, IPointerUpHandler, IPointerExitHandler
{
    public GameObject number_text; // Updated to match usage in code
    public List<GameObject> number_notes;
    private bool note_active;
    private int number_ = 0;
    public int correct_number_ = 0;

    private bool selected_=false;
    private int square_index_ = -1;
    private bool has_default_value_ = false;
    private bool has_wrong_value_ = false;
    

    public int GetSquareNumber()
    {
        return number_;
    }
    public bool IsCorrectNumberSet()
    {
        return number_ == correct_number_;
    }

    public bool HasWrongValue()
    {
        return has_wrong_value_;
    }
    public void SetHasDefaultValue(bool has_default)
    {
        has_default_value_= has_default;
    }
    public bool GetHasDefaultValue() { return has_default_value_; }
    public bool IsSelected()
        { return selected_; }
    public void SetSquareIndex(int index)
    {
        square_index_ = index;
    }
    public void SetCorrectNumber(int number)
    {
        correct_number_ = number;
        has_wrong_value_= false;
    }
    public void SetCorrectNumber()
    {
        number_ = correct_number_;
        SetNoteNumberValue(0);
        DisplayText();

    }
    void Start()
    {
        selected_ = false;
        note_active = false;

        SetNoteNumberValue(0);

    }
    public List<string> GetSquareNotes()
    {
        List<string> notes = new List<string>();
        
        foreach(var number in number_notes)
        {
            notes.Add(number.GetComponent<Text>().text);
        }
        return notes;
    }
    private void SetClearEmptynotes()
    {
        foreach (var number in number_notes)
        {
            if(number.GetComponent<Text>().text == "0")
                number.GetComponent<Text>().text = " ";
        }
    }
    private void SetNoteNumberValue(int value)
    {
        foreach(var number in number_notes)
        {
            if (value <= 0)
                number.GetComponent<Text>().text = " ";
            else
                number.GetComponent<Text>().text = value.ToString();
        }
    }

    private void SetNotesSingleNumberValue(int value, bool force_update = false)
    {
        if (note_active == false && force_update == false)
            return;

        if (value <= 0)
            number_notes[value - 1].GetComponent<Text>().text = " ";
        else
        {
            if (number_notes[value - 1].GetComponent<Text>().text == " " || force_update)
                number_notes[value - 1].GetComponent<Text>().text = value.ToString();
            else
                number_notes[value - 1].GetComponent<Text>().text = " ";
        }
    }

    public void SetGridNotes(List<int> notes)
    {
        foreach(var note in notes)
        {
            SetNotesSingleNumberValue(note, true);
        }
    }
    public void OnNotesActive(bool active)
    {
        note_active = active;
    }
    void Update()
    {

    }
    public void DisplayText()
    {
        if (number_text == null)
        {
            Debug.LogError("Number_text is not assigned in GridSquare.");
            return;
        }

        if (number_ <= 0)
            number_text.GetComponent<Text>().text = " ";
        else
            number_text.GetComponent<Text>().text = number_.ToString();
        if (has_default_value_)
        {
            number_text.GetComponent<Text>().fontStyle = FontStyle.Bold;
        }
    
    }

    public void SetNumber(int number)
    {
        number_ = number;
        DisplayText();
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        selected_=true;
        GameEvents.SquareSelectedMethod(square_index_);
        
    }

    public void OnSubmit(BaseEventData eventData)
    {

    }
    private void OnEnable()
    {
        GameEvents.OnUpdateSquareNumber += OnSetNumber;
        GameEvents.OnSquareSelected += OnSquareSelected;
        GameEvents.OnNotesActive += OnNotesActive;
        GameEvents.OnClearNumber += OnClearNumber;
        GameEvents.OnHint += OnHint;
    }
    private void OnDisable()
    {
        GameEvents.OnUpdateSquareNumber -= OnSetNumber;
        GameEvents.OnSquareSelected -= OnSquareSelected;
        GameEvents.OnNotesActive -= OnNotesActive;
        GameEvents.OnClearNumber -= OnClearNumber;
        GameEvents.OnHint -= OnHint;
    }
    public void OnHint()
    {
        if (selected_ && !has_default_value_)
        {
            number_ = correct_number_;
            //has_wrong_value_ = false;
            SetSquareColour(Color.white);
            //SetNoteNumberValue(correct_number_);
            DisplayText();
        }
    }
    public void OnGameOver()
    {
        if(number_ != 0 && number_ != correct_number_)
        {
            number_ = 0;
            DisplayText();
        }
        SetSquareColour(Color.white);
    }
    public void OnClearNumber()
    {
        if (selected_ && !has_default_value_)
        {
            number_ = 0;
            has_wrong_value_ =false;
            SetSquareColour(Color.white);
            SetNoteNumberValue(0);
            DisplayText();
        }
    }
    public void OnSetNumber(int number)
    {
        if(selected_ && has_default_value_==false)
        {
            if (note_active == true && has_default_value_ == false)
            {
                SetNotesSingleNumberValue(number);
            }
            else if (note_active == false)
            {
                SetNoteNumberValue(0);
                SetNumber(number);

                if (number_ != correct_number_)
                {
                    has_wrong_value_ = true;
                    var colors = this.colors;
                    colors.normalColor = Color.red;
                    this.colors = colors;

                    GameEvents.OnWrongNumberMethod();
                }
                else
                {
                    has_wrong_value_ = false;
                    has_default_value_ = true;
                    var color = this.colors;
                    color.normalColor = Color.white;
                    this.colors = color;
                }
            }
            GameEvents.CheckBoardCompletedMethod();
        }
    }

    public void OnSquareSelected(int squareIndex)
    {
        if (square_index_ != squareIndex)
        {
            selected_ = false;
        }
      
    }


    public void SetSquareColour(Color col)
    {
        var colors = this.colors;
        colors.normalColor = col;
        this.colors = colors;
        DoStateTransition(SelectionState.Normal, true);
    }

}
