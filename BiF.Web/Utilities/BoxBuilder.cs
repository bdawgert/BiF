using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BiF.DAL.Models;

namespace BiF.Web.Utilities
{
    public class BoxBuilder
    {

        private double? _minOunces;
        private double? _minRating;
        private double? _minCost;

        public List<Item> Items { get; set; }

        public BoxBuilder AddMinimumOunces(double minOunces) {
            _minOunces = minOunces;
            return this;
        }

        public BoxBuilder AddMinimumCost(double minCost) {
            _minCost = minCost;
            return this;
        }

        public BoxBuilder AddMinimumRating(double minRating) {
            _minRating = minRating;
            return this;
        }

        public BoxBuilder RemoveMinimumOunces() {
            _minOunces = null;
            return this;
        }

        public BoxBuilder RemoveMinimumCost() {
            _minCost = null;
            return this;
        }

        public BoxBuilder RemoveMinimumRating() {
            _minRating = null;
            return this;
        }
        
        private bool? validateOunces => Items.Where(x => x.Type == "Beer").Sum(x => x.USOunces) > _minOunces;
        private bool? validateCost => Items.Sum(x => x.Cost) > _minCost;
        private bool? validateRating => Items.Where(x => x.UntappdId != null).Sum(x => x.UntappdRating) > _minRating;

        public object Summarize() {
            double? totalOunces = Items.Where(x => x.Type == "Beer").Sum(x => x.USOunces);
            double? totalCost = Items.Sum(x => x.Cost);
            double? totalRating = Items.Where(x => x.UntappdId != null).Sum(x => x.UntappdRating * x.USOunces) / totalOunces;

            return null;
        }

    }
}