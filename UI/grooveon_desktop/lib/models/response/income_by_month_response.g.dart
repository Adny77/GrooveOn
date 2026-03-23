// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'income_by_month_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

IncomeByMonthResponse _$IncomeByMonthResponseFromJson(
  Map<String, dynamic> json,
) => IncomeByMonthResponse(
  month: (json['month'] as num).toInt(),
  totalIncome: (json['totalIncome'] as num).toDouble(),
);

Map<String, dynamic> _$IncomeByMonthResponseToJson(
  IncomeByMonthResponse instance,
) => <String, dynamic>{
  'month': instance.month,
  'totalIncome': instance.totalIncome,
};
