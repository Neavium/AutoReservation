﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoReservation.Common.DataTransferObjects;
using AutoReservation.Common.Extensions;
using AutoReservation.Common.Interfaces;
using AutoReservation.Wpf.Logic;
using AutoReservation.Service.Wcf;

using System.ServiceModel;


namespace AutoReservation.Wpf.Model {    
    public class AutoReservationRepository : INotifyPropertyChanged {
        private readonly ObservableCollection<ChangeTracker<AutoDto>> _autos;
		private readonly ObservableCollection<ChangeTracker<KundeDto>> _kunden;
        private readonly ObservableCollection<ChangeTracker<ReservationDto>> _reservationen;
        public ObservableCollection<ChangeTracker<AutoDto>> Autos => _autos; //.Select(auto => auto.Current).ToList();
		public ObservableCollection<ChangeTracker<KundeDto>> Kunden => _kunden;
        public ObservableCollection<ChangeTracker<ReservationDto>> Reservationen => _reservationen;

        private IAutoReservationService target;

        public AutoReservationRepository(string serverUrl = null) {

            if (target == null)
            {
                ChannelFactory<IAutoReservationService> channelFactory = new ChannelFactory<IAutoReservationService>("AutoReservationService");
                target = channelFactory.CreateChannel();
            }
            
            // TODO: Repair Real connection
            _autos = new ObservableCollection<ChangeTracker<AutoDto>>(target.GetAllCars().Select(auto => new ChangeTracker<AutoDto>(auto)));
			_kunden = new ObservableCollection<ChangeTracker<KundeDto>>(target.GetAllCustomers().Select(customer => new ChangeTracker<KundeDto>(customer)));
            _reservationen = new ObservableCollection<ChangeTracker<ReservationDto>>(target.GetAllReservations().Select(reservation => new ChangeTracker<ReservationDto>(reservation)));
        }

		#region Car Functions
		private void AddCar(AutoDto car) {
            try {
                car.Id = target.AddCar(car);
            } catch (Exception e) {
                throw new RepositoryException("Auto konnte nicht hinzugefügt werden.", e);
            }
        }

        private void UpdateCar(AutoDto car)
        {
            try
            {
                target.UpdateCar(car);
            }
            catch (Exception e)
            {
                throw new RepositoryException("Auto konnte nicht geändert werden.", e);
            }
        }

        private void DeleteCar(AutoDto car)
        {
            try
            {
                target.DeleteCar(car);
            }
            catch (Exception e)
            {
                throw new RepositoryException("Auto konnte nicht gelöscht werden.", e);
            }
        }

        public void SaveCarChanges()
        {
            _autos.Where(auto => auto.IsDirty).ToList().ForEach(auto => {
                if (auto.IsNew && auto.IsDeleted) {
                    _autos.Remove(auto);
                } else if (auto.IsNew) {
                    AddCar(auto.Current);
                    auto.IsNew = false;
                } else if (auto.IsDeleted)
                {
                    DeleteCar(auto.Current);
                    _autos.Remove(auto);
                } else {
                    UpdateCar(auto.Current);
                }
                auto.IsDirty = false;
            });
        }
		#endregion

		#region Customer Functions
		private void AddCustomer(KundeDto customer)
		{
			try
			{
				target.AddCustomer(customer);
			}
			catch (Exception e)
			{
				throw new RepositoryException("Kunde konnte nicht hinzugefügt werden.", e);
			}
		}

		private void UpdateCustomer(KundeDto customer)
		{
			try
			{
				target.UpdateCustomer(customer);
			}
			catch (Exception e)
			{
				throw new RepositoryException("Kunde konnte nicht geändert werden.", e);
			}
		}

		private void DeleteCustomer(KundeDto customer)
		{
			try
			{
				target.DeleteCustomer(customer);
			}
			catch (Exception e)
			{
				throw new RepositoryException("Kunde konnte nicht gelöscht werden.", e);
			}
		}

		public void SaveCustomerChanges()
		{
			_kunden.Where(customer => customer.IsDirty).ToList().ForEach(customer => {
				if (customer.IsNew && customer.IsDeleted)
				{
					_kunden.Remove(customer);
				}
				else if (customer.IsNew)
				{
					AddCustomer(customer.Current);
					customer.IsNew = false;
				}
				else if (customer.IsDeleted)
				{
					DeleteCustomer(customer.Current);
					_kunden.Remove(customer);
				}
				else
				{
					UpdateCustomer(customer.Current);
				}
				customer.IsDirty = false;
			});
		}
		#endregion

		#region Reservation Functions

		#endregion


		public void SaveAllChanges()
        {
            SaveCarChanges();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

