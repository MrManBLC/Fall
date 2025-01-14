﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fall.src;
using New_Physics.Traits;
using New_Physics.Entities;
using Microsoft.Xna.Framework;
using Fall.src.Entities;

namespace Fall.src.Traits
{

    public class FallingCollision : Trait
    {
        Entity parent;
        public Boolean isFalling;
        public string collidingWith = "nothing";
        int collidingIndex = -1;

        new List<Vector2> nc = new List<Vector2>();

        public FallingCollision(Entity parent) : base("fallingCollision", parent)
        {
            List<Vector2> nc = new List<Vector2>();
            nc.Add(new Vector2(0, 0));
            Init(parent, false, nc);
        }
        public FallingCollision(Entity parent, Boolean isFalling) : base("fallingCollision", parent)
        {
            List<Vector2> nc = new List<Vector2>();
            nc.Add(new Vector2(0, 0));
            nc.Add(new Vector2(parent.width, 0));
            Init(parent, isFalling, nc);
        }

        private void Init(Entity parent, Boolean isFalling, List<Vector2> nc)
        {
            this.parent = parent;
            this.isFalling = isFalling;
            this.nc = nc;
        }

        public override void Update()
        {
            if (isFalling)
            {
                collidingWith = "nothing";
                Boolean isVertical = false;
                if (parent.x - (parent.x + parent.dx) == 0)
                {
                    isVertical = true;
                }
                //ISN"T VERTICAL
                if (!isVertical)
                {
                    float mySlope = (parent.y - (parent.y + parent.dy)) / (parent.x - (parent.x + parent.dx));
                    float myB = parent.y - mySlope * parent.x;

                    for (int i = 0; i < EntityHandler.entities.Count; i++)
                    {
                        Entity entity = EntityHandler.entities[i];
                        if (!entity.hasTrait("fallingCollision") ||
                            ((FallingCollision)entity.getTrait("fallingCollision")).isFalling)
                        {
                            continue;
                        }

                        float pOI = (entity.y - myB) / mySlope;
                        Boolean isIntersecting = false;
                        //Check x
                        if (pOI > entity.x && pOI < entity.x + entity.width)
                        {
                            if ((parent.y > entity.y && parent.y + parent.dy < entity.y) ||
                                (parent.y + parent.dy > entity.y && parent.y < entity.y))
                            {
                                isIntersecting = true;
                            }
                        }

                        //Handle Intersecting Behavior
                        if (isIntersecting)
                        {
                            collidingWith = entity.classId;
                            collidingIndex = i;
                            parent.x = pOI;
                            parent.y = entity.y;
                            //parent.dy = 0;
                        }
                    }
                    return;
                }
                //IS VERTICAL
                for (int i = 0; i < EntityHandler.entities.Count; i++)
                {
                    Entity entity = EntityHandler.entities[i];
                    if (!entity.hasTrait("fallingCollision") ||
                        ((FallingCollision)entity.getTrait("fallingCollision")).isFalling)
                    {
                        continue;
                    }

                    Boolean isIntersecting = false;
                    if (parent.x > entity.x && parent.x < entity.x + entity.width)
                    {
                        if ((parent.y > entity.y && parent.y + parent.dy < entity.y) ||
                            (parent.y + parent.dy > entity.y && parent.y < entity.y))
                        {
                            isIntersecting = true;
                        }
                    }

                    if (isIntersecting)
                    {
                        collidingWith = entity.classId;
                        collidingIndex = i;
                        parent.y = entity.y;
                        //parent.dy = 0;
                    }
                }

                //Collision Reactions
                if (collidingWith == "leaf")
                {
                    parent.dy = -100*Camera.gameScale*((Gravity)((Player)parent).getTrait("gravity")).weight;
                    ((Player)parent).angleChange = new Random().Next(-20, 20);
                }
                else if (collidingWith == "bug")
                {
                    parent.dy = -100 * Camera.gameScale * ((Gravity)((Player)parent).getTrait("gravity")).weight; ;
                    ((Bug)EntityHandler.entities[collidingIndex]).JumpedOn();
                    ((Player)parent).angleChange = new Random().Next(-20, 20);
                }
                else if (collidingWith == "branch")
                {
                    parent.dy = 0;
                    ((Player)parent).isAlive = false;
                }

                return;
            }
        }
    }
}
