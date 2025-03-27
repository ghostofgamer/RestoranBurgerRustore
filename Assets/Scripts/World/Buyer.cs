using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TheSTAR.Utility;
using Zenject;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace World
{
    public class Buyer : MonoBehaviour, IQueueMember
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private NavMeshObstacle obstacle;
        [SerializeField] private Transform bubblePos;
        [SerializeField] private Animator anim;
        [SerializeField] private Image _waitImage;
        [SerializeField] private Image _waitBackGroundImage;
        [SerializeField] private Sprite[] _smiles;
        [SerializeField] private Image _smileEmotion;

        private int _rateWaitingOrder;
        private int _polluteRate;
        private int _indexEmotion;
        private bool _isWait = false;
        private float _elapsedTime;
        private Coroutine animationCoroutine;
        private Color endColor = Color.red;
        private Color startColor = Color.green;
        private float _duration = 165.0f;
        private bool _isWaiting;


        public int RateWaitingOrder => _rateWaitingOrder;
        public int PolluteRate => _polluteRate;


        [Space]
        //[SerializeField] private TouchInteractive touchInteractive;
        [SerializeField]
        private Dragger dragger;

        [SerializeField] private TutorInWorldFocus tutorFocus;

        private const string Eat_FoodAnimKey = "Eat";
        private const string Eat_DrinkAnimKey = "Drink";

        private OrderData orderData;

        public PaymentType PaymentType
        {
            get
            {
                var allPaymentTypes = EnumUtility.GetValues<PaymentType>();
                return ArrayUtility.GetRandomValue(allPaymentTypes);
            }
        }

        public OrderData OrderData => orderData;
        public Transform BubblePos => bubblePos;

        private BuyerCar car;
        public BuyerCar Car => car;

        private BuyerPlace place;
        public BuyerPlace Place => place;
        public TutorInWorldFocus TutorFocus => tutorFocus;

        public event Action<Buyer> OnSitEvent;
        public event Action<Buyer> OnCompleteSitEvent;
        public event Action<Buyer, int> OnChangeQueueIndexEvent;

        public event Action<Buyer>
            OnGiveItemToOrderEvent; // был выдан предмет по заказу (аргументом передаётся что в итоге стало с заказом)

        private int queueIndex;
        public int QueueIndex => queueIndex;

        private GameWorldInteraction gameWorldInteraction;
        private BuyersController buyersController;
        private ItemsController items;

        [Inject]
        private void Construct(GameWorldInteraction gameWorldInteraction, BuyersController buyersController,
            ItemsController items)
        {
            this.gameWorldInteraction = gameWorldInteraction;
            this.buyersController = buyersController;
            this.items = items;
        }

        public void Init(BuyerCar car, OrderData orderData)
        {
            this.car = car;
            //touchInteractive.gameObject.SetActive(false);

            this.orderData = new(orderData);
            //touchInteractive.OnClickEvent += () => gameWorldInteraction.OnClickBuyer(this);
        }

        public void StartWait()
        {
            if (_isWait) return;

            _isWaiting = true;
            _isWait = true;
            _rateWaitingOrder = 0;
            _smileEmotion.gameObject.SetActive(true);
            _smileEmotion.sprite = _smiles[0]; 
            StartCoroutine(AnimateWaiting());


            /*_waitImage.gameObject.SetActive(true);
            _waitBackGroundImage.gameObject.SetActive(true);*/

            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);

            animationCoroutine = StartCoroutine(AnimateWaiting());
        }

        private IEnumerator AnimateWaiting()
        {
            _elapsedTime = 0;

            while (_elapsedTime < _duration && _isWaiting)
            {
                _elapsedTime += Time.deltaTime;
                float t = _elapsedTime / _duration;
                _smileEmotion.color = Color.Lerp(startColor, endColor, t);

                if (t <= 0.4f)
                {
                    _smileEmotion.sprite = _smiles[0];
                    _rateWaitingOrder = 1;
                }
                else if (t <= 0.7f)
                {
                    _rateWaitingOrder = 0;
                    _smileEmotion.sprite = _smiles[1];
                }
                else
                {
                    _rateWaitingOrder = -1;
                    _smileEmotion.sprite = _smiles[2];
                }

                yield return null;
            }
        }

        public void StopWaitingAnimation()
        {
            _isWaiting = false;

            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);
        }

        private PathPoint goal;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Point")) return;
            if (other.GetComponent<PathPoint>() != goal) return;

            OnReachGoal();
        }

        private void OnReachGoal()
        {
            anim.SetBool("Walking", false);
            agent.enabled = false;
            obstacle.enabled = true;
            transform.rotation = goal.transform.rotation;
            reachAction?.Invoke(this);
            reachAction = null;
            goal = null;
        }

        private Action<Buyer> reachAction;

        public void SetDestination(PathPoint goal, Action<Buyer> reachAction)
        {
            this.goal = goal;
            anim.SetBool("Walking", true);
            //lockReachForStartMove = true;

            obstacle.enabled = false;
            agent.enabled = true;
            agent.SetDestination(goal.transform.position);

            if (reachAction != null) this.reachAction = reachAction;
        }

        private bool active = true;
        public bool Active => active;

        /*
        public void Activate(OrderData order, Vector3 pos)
        {
            Init(order);
            transform.position = pos;
            active = true;
            touchInteractive.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
        */

        public void Deactivate()
        {
            active = false;
            gameObject.SetActive(false);
        }

        public void GiveItem(ItemType itemType)
        {
            orderData.AddItem(itemType);
            OnGiveItemToOrderEvent?.Invoke(this);
        }

        [ContextMenu("GiveAllNeededItems")]
        public void GiveAllNeededItems()
        {
            orderData.AddAllNeededItems();
            OnGiveItemToOrderEvent?.Invoke(this);
        }

        [ContextMenu("CheckIsOrderCompleted")]
        private void CheckIsOrderCompleted()
        {
            Debug.Log("OrderCompleted: " + IsOrderCompleted());
        }

        public bool IsOrderCompleted() => orderData.IsOrderCompleted();

        //public void LookAtDefault() => LookAt(lookAtTran);
        private void LookAt(Transform at)
        {
            var direction = Vector3.Normalize(at.position - transform.position);
            var lookRotation = Quaternion.LookRotation(direction);
            var euler = lookRotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, euler.y, 0);
        }

        public void ChangeQueueIndex(int index)
        {
            Debug.Log("ChangeQueueIndex");
            this.queueIndex = index;
            OnChangeQueueIndexEvent?.Invoke(this, index);
        }

        public void ReservePlace(BuyerPlace place)
        {
            this.place = place;
        }

        public void Sit(BuyerPlace place)
        {
            Debug.Log("Sit");

            this.place = place;

            agent.enabled = false;
            transform.position = place.SittingPlace.position;
            transform.rotation = place.SittingPlace.rotation;
            anim.SetBool("Sit", true);

            //touchInteractive.gameObject.SetActive(true);

            OnSitEvent?.Invoke(this);
        }

        private bool finish = false;
        public bool Finish => finish;

        public void FinishSit(Transform placeToStand)
        {
            Debug.Log("FinishSit ");
            Debug.Log("Rate Waiting  " + _rateWaitingOrder);

            finish = true;
            agent.enabled = true;
            transform.position = placeToStand.position;
            transform.rotation = placeToStand.rotation;
            anim.SetBool("Sit", false);

            //touchInteractive.gameObject.SetActive(false);

            OnCompleteSitEvent?.Invoke(this);
        }

        private Tweener eatingTweener;

        private bool inEating = false;
        public bool InEating => inEating;

        public void StartEat(float duration)
        {
            Debug.Log("StartEat");

            inEating = true;
            //touchInteractive.gameObject.SetActive(false);
            place.OnBuyerStartEat();

            eatingTweener?.Kill();

            eatingTweener = DOVirtual.Float(0f, 1f, duration, (temp) => { }).SetEase(Ease.Linear).OnComplete(() =>
            {
                CompleteEat();
            });

            place.CheckItems(out var haveDrink, out var haveFood);

            string key;
            List<string> animKeys = new();

            if (haveDrink) key = Eat_DrinkAnimKey;
            else
            {
                if (haveFood) animKeys.Add(Eat_FoodAnimKey);
                key = ArrayUtility.GetRandomValue(animKeys.ToArray());
            }

            if (key == Eat_DrinkAnimKey)
            {
                foreach (var element in place.OrderItems)
                {
                    if (element == null) continue;
                    var item = element.GetComponent<Item>();
                    if (item == null) continue;
                    if (items.GetItemData(item.ItemType).MainData.SectionType == ItemSectionType.FinalDrink)
                    {
                        item.Draggable.CurrentDragger.EndDrag();
                        dragger.StartDrag(item.Draggable);
                        break;
                    }
                }
            }

            anim.SetBool(key, true);
        }

        private void CompleteEat()
        {
            Debug.Log("CompleteEat");
            
            _polluteRate = Place.PollutionLevel == 0 ? 1 : -Place.PollutionLevel;
            Debug.Log("Загрязнение стола " + _polluteRate);

            Place.PolluteTable();
            Debug.Log("А урвоень ожидания  " + _rateWaitingOrder);

            inEating = false;
            anim.SetBool(Eat_DrinkAnimKey, false);
            anim.SetBool(Eat_FoodAnimKey, false);

            var itemInHand = dragger.CurrentDraggable;
            if (itemInHand != null)
            {
                dragger.EndDrag();
                Destroy(itemInHand.gameObject);
            }

            Place.ClearOrderItems();
            Place.ClearBuyer();
            buyersController.GoAway(this);
        }
    }
}

public enum PaymentType
{
    DebitCard,
    Cash
}