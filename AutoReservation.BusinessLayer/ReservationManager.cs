﻿using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal;
using AutoReservation.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoReservation.BusinessLayer
{
	public class ReservationManager
		: ManagerBase
	{
		public List<Reservation> ListOfReservationen
		{
			get
			{
				using (AutoReservationContext context = new AutoReservationContext())
				{
					return context.Reservationen.Include(r => r.Auto).Include(r => r.Kunde).ToList();
				}
			}
		}

		public Reservation GetReservation(int resNr)
		{
			using (AutoReservationContext context = new AutoReservationContext())
			{
				return context.Reservationen.Include(r => r.Auto).Include(r => r.Kunde)
					.Single(res => res.ReservationsNr == resNr);
			}
		}

		public int InsertReservation(Reservation reservation)
		{
			using (AutoReservationContext context = new AutoReservationContext())
			{
				if (!InRangeCheck(reservation))
				{
					throw new InvalidDateRangeException("Error while inserting", reservation);
				}
				else if (!CarAvailable(context, reservation))
				{
					throw new AutoUnavailableException("Error while inserting", reservation);
				}
				else
				{
					context.Entry(reservation).State = EntityState.Added;
					context.SaveChanges();
                    return reservation.ReservationsNr;
				}
			}
		}



		public bool UpdateReservation(Reservation reservation)
		{
			using (AutoReservationContext context = new AutoReservationContext())
			{
				if (!InRangeCheck(reservation))
				{
					throw new InvalidDateRangeException("Error while inserting", reservation);
				}
				else if (!CarAvailable(context, reservation))
				{
					throw new AutoUnavailableException("Error while inserting", reservation);
				}
                else
                { 
                    if(context.Entry(reservation) != null)
                    {
                        context.Entry(reservation).State = EntityState.Modified;
					    try
					    {
						    context.SaveChanges();
                            return true;
					    }
					    catch (DbUpdateConcurrencyException)
					    {
						    throw CreateOptimisticConcurrencyException(context, reservation);
					    }
                    }
                    else
                    {
                        return false;
                    }
				}
			}
		}

		public bool DeleteReservation(Reservation reservation)
		{
			using (AutoReservationContext context = new AutoReservationContext())
			{
                if(context.Entry(reservation) != null)
                {
                    context.Entry(reservation).State = EntityState.Deleted;
				    try
				    {
					    context.SaveChanges();
                        return true;
				    }
				    catch (Exception e)
				    {
					    Console.WriteLine(e);
					    throw;
				    }
                }
                else
                {
                    return false;
                }
				
			}
		}

		public bool InRangeCheck(Reservation reservation)
		{
            return (reservation.Von.AddHours(24).CompareTo(reservation.Bis) <= 0);
		}

		private bool CarAvailable(AutoReservationContext context, Reservation reservation)
		{
            return !context.Reservationen
                .Where(res => res.ReservationsNr != reservation.ReservationsNr)
				.Where(res => res.AutoId == reservation.AutoId)
				.Where(res => res.Von >= reservation.Von && res.Von < reservation.Bis && res.Bis >= reservation.Bis
                            || res.Bis > reservation.Von && res.Bis <= reservation.Bis && res.Von <= reservation.Von
                            || res.Von <= reservation.Von && res.Bis >= reservation.Bis
                            || res.Von >= reservation.Von && res.Bis <= reservation.Bis).Any();
		}
	}
}