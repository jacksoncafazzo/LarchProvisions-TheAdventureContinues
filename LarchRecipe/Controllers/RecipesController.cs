﻿using LarchRecipe.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LarchRecipe.Controllers
{
    public class RecipesController : Controller
    {
        private RecipesDBContext db = new RecipesDBContext();

        private Repository _repository = new Repository();

        // GET: Recipes
        public ActionResult Index()
        {
            return View(db.Recipe.ToList());
        }

        // GET: Recipes/Create
        public ActionResult Create()
        {
            ViewBag.Recipes = _repository.GetRecipes();
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,Notes,Servings")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Recipe.Add(recipe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipe.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            ViewBag.Message = recipe.Name;
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,Notes,Servings")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipe.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Recipe recipe = db.Recipe.Find(id);
            db.Recipe.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /*
        // Get: Recipes/Ingredients/1
        public ActionResult Ingredients(int? id)
        {
            //Repository viewmodel = new Repository();

            Recipe selectedRecipe = db.Recipe.Find(id);
            ViewBag.Recipe = selectedRecipe;

            var ingredients = from i in db.Ingredients
                              select i;
            ingredients = ingredients.Where(i => i.RecipeId == id);
            ViewBag.Ingredients = ingredients;
            return View("Index", "Ingredients", ingredients);
        }
        */

        //Get: Recipes/Details/1
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ingredients = from i in db.Ingredients
                              select i;
            ingredients = ingredients.Where(i => i.RecipeId == id);
            ViewBag.Ingredients = ingredients;
            ViewBag.Recipe = db.Recipe.Find(id);
            IEnumerable<Recipe> recipes = db.Recipe.ToList();
            ViewBag.Recipes = recipes;
            IEnumerable<SelectListItem> RecipeList =
                from r in recipes
                select new SelectListItem
                {
                    Selected = (r.ID == id),
                    Text = r.Name,
                    Value = r.ID.ToString()
                };

            ViewBag.RecipeList = RecipeList;

            return View();
        }

        //Post: Recipes/Details/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "ID, Name, Source, Amount, Unit, RecipeId")] Ingredient ingredient)
        {
            Recipe recipe = new Recipe();
            recipe = db.Recipe.Find(ingredient.RecipeId);
            if (ModelState.IsValid)
            {
                ingredient.RecipeName = recipe.Name;
                db.Ingredients.Add(ingredient);
                db.SaveChanges();
                var ingredients = from i in db.Ingredients
                                  select i;
                ingredients = ingredients.Where(i => i.RecipeId == ingredient.RecipeId);
                ViewBag.Ingredients = ingredients;
                ViewBag.Recipe = recipe;
                ViewBag.Recipes = db.Recipe.ToList();
                List<SelectListItem> RecipeList = new List<SelectListItem>();
                foreach (Recipe rec in ViewBag.Recipes)
                {
                    RecipeList.Add(new SelectListItem { Text = rec.Name, Value = rec.ID.ToString() });
                }
                ViewBag.RecipeList = RecipeList;
                return View();
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}